using System;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using BloodDonorManagementSystem.Infrastructure;

namespace BloodDonorManagementSystem
{
    public partial class DonorRegistration : Page
    {
        private const int MinimumDonorAge = 18;
        private const int MaximumDonorAge = 65;

        // =========================================================
        // PAGE / USER STATE
        // =========================================================

        private int EditDonorId
        {
            get
            {
                int id;

                return int.TryParse(
                    Request.QueryString["id"],
                    out id) && id > 0
                    ? id
                    : 0;
            }
        }

        private bool IsAdminUser
        {
            get
            {
                return AuthHelper.IsLoggedIn &&
                       AuthHelper.IsAdmin;
            }
        }

        private bool IsDonorUser
        {
            get
            {
                return AuthHelper.IsLoggedIn &&
                       AuthHelper.IsDonor;
            }
        }

        private bool IsAdminCreate
        {
            get
            {
                return IsAdminUser &&
                       EditDonorId <= 0;
            }
        }

        private bool IsAdminEdit
        {
            get
            {
                return IsAdminUser &&
                       EditDonorId > 0;
            }
        }

        // =========================================================
        // PAGE LOAD
        // =========================================================

        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            AuthHelper.RequireLogin();

            if (!IsPostBack)
            {
                InitialisePage();
            }
        }

        // =========================================================
        // PAGE INITIALISATION
        // =========================================================

        private void InitialisePage()
        {
            HideMessages();

            if (IsAdminUser)
            {
                InitialiseAdminPage();
                return;
            }

            if (IsDonorUser)
            {
                InitialiseDonorPage();
                return;
            }

            ShowError(
                "You are not authorised to access this page.");

            DisableForm();
        }

        // =========================================================
        // ADMIN PAGE
        // =========================================================

        private void InitialiseAdminPage()
        {
            lnkCancel.NavigateUrl =
                "~/Donors.aspx";

            // -----------------------------------------------------
            // ADMIN CREATE
            // -----------------------------------------------------

            if (EditDonorId <= 0)
            {
                SetAdminCreateMode();

                // IMPORTANT:
                // Every new donor starts as UNAVAILABLE.
                //
                // Database value:
                // IsAvailable = 0
                //
                // Admin is allowed to submit this value during
                // creation, but the default is OFF.
                chkAvailable.Checked = false;
                chkAvailable.Enabled = true;

                return;
            }

            // -----------------------------------------------------
            // ADMIN EDIT
            // -----------------------------------------------------

            DonorData donor =
                LoadDonorById(EditDonorId);

            if (donor == null)
            {
                ShowError(
                    "The selected donor could not be found.");

                DisableForm();
                return;
            }

            if (donor.UserId <= 0)
            {
                ShowError(
                    "This donor is not linked to a user account.");

                DisableForm();
                return;
            }

            SetAdminEditMode(donor);

            // IMPORTANT:
            // Admin can VIEW availability.
            //
            // Admin CANNOT CHANGE availability.
            //
            // The value displayed here comes from the database.
            chkAvailable.Checked =
                donor.IsAvailable;

            chkAvailable.Enabled = false;
        }

        private void SetAdminCreateMode()
        {
            litPageTitle.Text =
                "Donor Registration";

            litPageSubtitle.Text =
                "Create a new donor registration and user account.";

            btnSave.Text =
                "Create Donor";

            chkAvailable.Enabled = true;
            chkAvailable.Checked = false;
        }

        private void SetAdminEditMode(
            DonorData donor)
        {
            litPageTitle.Text =
                "Edit Donor";

            litPageSubtitle.Text =
                "Update the donor's registration information.";

            btnSave.Text =
                "Update Donor";

            LoadDonorIntoForm(donor);

            // Admin edit:
            // availability is always read-only.
            chkAvailable.Enabled = false;
        }

        // =========================================================
        // DONOR PAGE
        // =========================================================

        private void InitialiseDonorPage()
        {
            lnkCancel.NavigateUrl =
                "~/DonorDashboard.aspx";

            // -----------------------------------------------------
            // SECURITY:
            //
            // The QueryString donor ID is completely ignored
            // for donor users.
            //
            // Donors always work with AuthHelper.UserId.
            // -----------------------------------------------------

            DonorData donor =
                LoadDonorByUserId(
                    AuthHelper.UserId);

            // -----------------------------------------------------
            // FIRST REGISTRATION
            // -----------------------------------------------------

            if (donor == null)
            {
                litPageTitle.Text =
                    "Donor Registration";

                litPageSubtitle.Text =
                    "Complete your donor registration information.";

                btnSave.Text =
                    "Save Registration";

                // IMPORTANT:
                // Initial donor registration is ALWAYS unavailable.
                //
                // The donor cannot register as immediately available.
                // After registration, they can edit their own record
                // and change availability.
                chkAvailable.Checked = false;

                chkAvailable.Enabled = false;

                return;
            }

            // -----------------------------------------------------
            // EXISTING DONOR
            // -----------------------------------------------------

            litPageTitle.Text =
                "My Donor Registration";

            litPageSubtitle.Text =
                "Update your personal donor information.";

            btnSave.Text =
                "Update Registration";

            LoadDonorIntoForm(donor);

            // IMPORTANT:
            // Existing donor may change their own availability.
            chkAvailable.Enabled = true;
        }

        // =========================================================
        // SAVE
        // =========================================================

        protected void btnSave_Click(
            object sender,
            EventArgs e)
        {
            HideMessages();

            if (!Page.IsValid)
            {
                return;
            }

            DateTime dateOfBirth;

            if (!TryGetDateOfBirth(
                out dateOfBirth))
            {
                ShowError(
                    "Please enter a valid date of birth.");

                return;
            }

            if (!IsValidAge(dateOfBirth))
            {
                ShowError(
                    "Donor age must be between 18 and 65 years.");

                return;
            }

            // -----------------------------------------------------
            // ADMIN
            // -----------------------------------------------------

            if (IsAdminUser)
            {
                SaveAsAdmin(dateOfBirth);
                return;
            }

            // -----------------------------------------------------
            // DONOR
            // -----------------------------------------------------

            if (IsDonorUser)
            {
                SaveAsDonor(dateOfBirth);
                return;
            }

            ShowError(
                "You are not authorised to perform this action.");
        }

        // =========================================================
        // ADMIN SAVE
        // =========================================================

        private void SaveAsAdmin(
            DateTime dateOfBirth)
        {
            if (EditDonorId > 0)
            {
                UpdateDonorAsAdmin(
                    EditDonorId,
                    dateOfBirth);

                return;
            }

            CreateDonorAsAdmin(
                dateOfBirth);
        }

        // =========================================================
        // DONOR SAVE
        // =========================================================

        private void SaveAsDonor(
            DateTime dateOfBirth)
        {
            // -----------------------------------------------------
            // ALWAYS LOAD BY LOGGED-IN USER
            // -----------------------------------------------------

            DonorData existing =
                LoadDonorByUserId(
                    AuthHelper.UserId);

            // -----------------------------------------------------
            // FIRST REGISTRATION
            // -----------------------------------------------------

            if (existing == null)
            {
                CreateDonorForExistingUser(
                    dateOfBirth);

                return;
            }

            // -----------------------------------------------------
            // SECURITY CHECK
            // -----------------------------------------------------

            if (existing.UserId !=
                AuthHelper.UserId)
            {
                ShowError(
                    "You are not authorised to edit this donor record.");

                return;
            }

            // -----------------------------------------------------
            // UPDATE OWN RECORD
            // -----------------------------------------------------

            UpdateDonorForOwner(
                existing,
                dateOfBirth);
        }

        // =========================================================
        // GET FORM DATA
        // =========================================================

        private DonorFormData GetFormData(
            DateTime dateOfBirth)
        {
            return new DonorFormData
            {
                FullName =
                    CleanText(
                        txtFullName.Text,
                        150),

                BloodGroup =
                    CleanText(
                        ddlBloodGroup.SelectedValue,
                        5),

                Gender =
                    CleanNullableText(
                        ddlGender.SelectedValue,
                        20),

                DateOfBirth =
                    dateOfBirth,

                Mobile =
                    CleanText(
                        txtMobile.Text,
                        20),

                Email =
                    CleanNullableText(
                        txtEmail.Text,
                        255),

                Address =
                    CleanNullableText(
                        txtAddress.Text,
                        500),

                City =
                    CleanText(
                        txtCity.Text,
                        100),

                State =
                    CleanNullableText(
                        txtState.Text,
                        100),

                Pincode =
                    CleanNullableText(
                        txtPincode.Text,
                        10),

                IsAvailable =
                    chkAvailable.Checked
            };
        }

        // =========================================================
        // ADMIN CREATE
        // =========================================================

        private void CreateDonorAsAdmin(
            DateTime dateOfBirth)
        {
            DonorFormData data =
                GetFormData(dateOfBirth);

            // IMPORTANT:
            // New donor registration starts unavailable.
            //
            // Admin may explicitly choose availability during
            // creation, therefore we do NOT overwrite the checkbox.
            //
            // Default page state is OFF.

            if (!ValidateBusinessData(data))
            {
                return;
            }

            string firstName;
            string lastName;

            SplitName(
                data.FullName,
                out firstName,
                out lastName);

            string username =
                GenerateUniqueUsername(
                    firstName,
                    lastName,
                    data.Mobile);

            string temporaryPassword =
                PasswordHelper.GenerateTemporaryPassword(10);

            string salt =
                PasswordHelper.GenerateSalt();

            string passwordHash =
                PasswordHelper.Hash(
                    temporaryPassword,
                    salt);

            try
            {
                using (SqlConnection connection =
                    Db.OpenConnection())
                using (SqlTransaction transaction =
                    connection.BeginTransaction())
                {
                    try
                    {
                        int roleId =
                            GetDonorRoleId(
                                connection,
                                transaction);

                        if (roleId <= 0)
                        {
                            throw new ApplicationException(
                                "The Donor role is not configured.");
                        }

                        int userId =
                            InsertUser(
                                connection,
                                transaction,
                                username,
                                passwordHash,
                                salt,
                                roleId,
                                firstName,
                                lastName,
                                data);

                        InsertDonor(
                            connection,
                            transaction,
                            userId,
                            data);

                        transaction.Commit();
                    }
                    catch
                    {
                        SafeRollback(transaction);
                        throw;
                    }
                }

                ShowSuccess(
                    "Donor registration completed successfully.");

                // =================================================
                // UPDATED:
                // Show generated donor credentials for 60 seconds.
                //
                // The previous 2-second redirect has intentionally
                // been removed.
                // =================================================

                ShowGeneratedCredentials(
                    data.FullName,
                    username,
                    temporaryPassword);

                // DO NOT add:
                //
                // RedirectAfterDelay(
                //     "~/Dashboard.aspx",
                //     2000);
                //
                // The credential countdown now handles the redirect.
            }
            catch (SqlException ex)
            {
                HandleDatabaseException(
                    ex,
                    "Unable to create the donor account.");
            }
            catch (Exception)
            {
                ShowError(
                    "Unable to create the donor account. Please try again.");
            }
        }

        // =========================================================
        // ADMIN UPDATE
        // =========================================================

        private void UpdateDonorAsAdmin(
            int donorId,
            DateTime dateOfBirth)
        {
            DonorData donor =
                LoadDonorById(donorId);

            if (donor == null)
            {
                ShowError(
                    "The donor could not be found.");

                return;
            }

            if (donor.UserId <= 0)
            {
                ShowError(
                    "The donor is not linked to a user account.");

                return;
            }

            DonorFormData data =
                GetFormData(dateOfBirth);

            // =====================================================
            // CRITICAL SECURITY RULE
            // =====================================================
            //
            // Never trust chkAvailable during admin edit.
            //
            // The availability value comes directly from the
            // database record that was loaded using donorId.
            //
            // Even if somebody manipulates the disabled checkbox
            // through browser developer tools, this value is
            // ignored.
            // =====================================================

            data.IsAvailable =
                donor.IsAvailable;

            if (!ValidateBusinessData(data))
            {
                return;
            }

            try
            {
                using (SqlConnection connection =
                    Db.OpenConnection())
                using (SqlTransaction transaction =
                    connection.BeginTransaction())
                {
                    try
                    {
                        UpdateUser(
                            connection,
                            transaction,
                            donor.UserId,
                            data);

                        UpdateDonorAsAdmin(
                            connection,
                            transaction,
                            donor.DonorId,
                            donor.UserId,
                            data);

                        transaction.Commit();
                    }
                    catch
                    {
                        SafeRollback(transaction);
                        throw;
                    }
                }

                ShowSuccess(
                    "Donor information updated successfully.");

                RedirectAfterDelay(
                    "~/Dashboard.aspx",
                    2000);
            }
            catch (SqlException ex)
            {
                HandleDatabaseException(
                    ex,
                    "Unable to update donor information.");
            }
            catch (Exception)
            {
                ShowError(
                    "Unable to update donor information. Please try again.");
            }
        }

        // =========================================================
        // CREATE DONOR FOR EXISTING LOGGED-IN USER
        // =========================================================

        private void CreateDonorForExistingUser(
            DateTime dateOfBirth)
        {
            DonorFormData data =
                GetFormData(dateOfBirth);

            // =====================================================
            // CRITICAL RULE
            // =====================================================
            //
            // A donor's FIRST registration is ALWAYS unavailable.
            //
            // Do not trust the checkbox here.
            // =====================================================

            data.IsAvailable = false;

            if (!ValidateBusinessData(data))
            {
                return;
            }

            try
            {
                using (SqlConnection connection =
                    Db.OpenConnection())
                using (SqlTransaction transaction =
                    connection.BeginTransaction())
                {
                    try
                    {
                        // Update the already authenticated donor user.
                        UpdateUser(
                            connection,
                            transaction,
                            AuthHelper.UserId,
                            data);

                        // Create exactly ONE donor record for the
                        // authenticated user.
                        InsertDonor(
                            connection,
                            transaction,
                            AuthHelper.UserId,
                            data);

                        transaction.Commit();
                    }
                    catch
                    {
                        SafeRollback(transaction);
                        throw;
                    }
                }

                ShowSuccess(
                    "Donor registration completed successfully.");

                RedirectAfterDelay(
                    "~/DonorDashboard.aspx",
                    2000);
            }
            catch (SqlException ex)
            {
                HandleDatabaseException(
                    ex,
                    "Unable to save your donor registration.");
            }
            catch (Exception)
            {
                ShowError(
                    "Unable to save your donor registration. Please try again.");
            }
        }

        // =========================================================
        // DONOR UPDATE OWN RECORD
        // =========================================================

        private void UpdateDonorForOwner(
            DonorData donor,
            DateTime dateOfBirth)
        {
            // =====================================================
            // CRITICAL SECURITY CHECK
            // =====================================================

            if (donor == null)
            {
                ShowError(
                    "The donor record could not be found.");

                return;
            }

            if (donor.UserId !=
                AuthHelper.UserId)
            {
                ShowError(
                    "You are not authorised to edit this donor record.");

                return;
            }

            DonorFormData data =
                GetFormData(dateOfBirth);

            // =====================================================
            // OWNER UPDATE
            // =====================================================
            //
            // This is the ONLY normal donor operation where
            // availability is taken from the checkbox.
            //
            // The donor is editing their OWN existing record.
            //
            // Therefore:
            //
            // chkAvailable = true  -> IsAvailable = 1
            // chkAvailable = false -> IsAvailable = 0
            // =====================================================

            if (!ValidateBusinessData(data))
            {
                return;
            }

            try
            {
                using (SqlConnection connection =
                    Db.OpenConnection())
                using (SqlTransaction transaction =
                    connection.BeginTransaction())
                {
                    try
                    {
                        UpdateUser(
                            connection,
                            transaction,
                            AuthHelper.UserId,
                            data);

                        UpdateDonorForOwner(
                            connection,
                            transaction,
                            donor.DonorId,
                            AuthHelper.UserId,
                            data);

                        transaction.Commit();
                    }
                    catch
                    {
                        SafeRollback(transaction);
                        throw;
                    }
                }

                ShowSuccess(
                    "Your donor registration was updated successfully.");

                RedirectAfterDelay(
                    "~/DonorDashboard.aspx",
                    2000);
            }
            catch (SqlException ex)
            {
                HandleDatabaseException(
                    ex,
                    "Unable to update your donor registration.");
            }
            catch (Exception)
            {
                ShowError(
                    "Unable to update your donor registration. Please try again.");
            }
        }

        // =========================================================
        // INSERT USER
        // =========================================================

        private int InsertUser(
            SqlConnection connection,
            SqlTransaction transaction,
            string username,
            string passwordHash,
            string passwordSalt,
            int roleId,
            string firstName,
            string lastName,
            DonorFormData data)
        {
            const string sql = @"
                INSERT INTO dbo.Users
                (
                    Username,
                    PasswordHash,
                    PasswordSalt,
                    RoleId,
                    FirstName,
                    LastName,
                    Email,
                    Phone,
                    IsActive,
                    IsEmailVerified,
                    IsPhoneVerified,
                    CreatedDate,
                    UpdatedDate,
                    RoleName,
                    MustChangePassword,
                    FullName
                )
                OUTPUT INSERTED.UserId
                VALUES
                (
                    @Username,
                    @PasswordHash,
                    @PasswordSalt,
                    @RoleId,
                    @FirstName,
                    @LastName,
                    @Email,
                    @Phone,
                    1,
                    0,
                    0,
                    SYSDATETIME(),
                    SYSDATETIME(),
                    'Donor',
                    1,
                    @FullName
                );";

            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection,
                    transaction))
            {
                command.Parameters.Add(
                    "@Username",
                    SqlDbType.NVarChar,
                    100).Value =
                    username;

                command.Parameters.Add(
                    "@PasswordHash",
                    SqlDbType.NVarChar,
                    500).Value =
                    passwordHash;

                command.Parameters.Add(
                    "@PasswordSalt",
                    SqlDbType.NVarChar,
                    500).Value =
                    passwordSalt;

                command.Parameters.Add(
                    "@RoleId",
                    SqlDbType.Int).Value =
                    roleId;

                command.Parameters.Add(
                    "@FirstName",
                    SqlDbType.NVarChar,
                    100).Value =
                    firstName;

                command.Parameters.Add(
                    "@LastName",
                    SqlDbType.NVarChar,
                    100).Value =
                    ToDbValue(lastName);

                command.Parameters.Add(
                    "@Email",
                    SqlDbType.NVarChar,
                    150).Value =
                    ToDbValue(
                        Truncate(
                            data.Email,
                            150));

                command.Parameters.Add(
                    "@Phone",
                    SqlDbType.NVarChar,
                    20).Value =
                    data.Mobile;

                command.Parameters.Add(
                    "@FullName",
                    SqlDbType.NVarChar,
                    150).Value =
                    data.FullName;

                object result =
                    command.ExecuteScalar();

                if (result == null ||
                    result == DBNull.Value)
                {
                    throw new ApplicationException(
                        "Unable to create the donor user account.");
                }

                return Convert.ToInt32(result);
            }
        }

        // =========================================================
        // UPDATE USER
        // =========================================================

        private void UpdateUser(
            SqlConnection connection,
            SqlTransaction transaction,
            int userId,
            DonorFormData data)
        {
            string firstName;
            string lastName;

            SplitName(
                data.FullName,
                out firstName,
                out lastName);

            const string sql = @"
                UPDATE dbo.Users
                SET
                    FirstName = @FirstName,
                    LastName = @LastName,
                    Email = @Email,
                    Phone = @Phone,
                    FullName = @FullName,
                    UpdatedDate = SYSDATETIME()
                WHERE UserId = @UserId
                  AND IsActive = 1
                  AND RoleName = 'Donor';";

            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection,
                    transaction))
            {
                command.Parameters.Add(
                    "@FirstName",
                    SqlDbType.NVarChar,
                    100).Value =
                    firstName;

                command.Parameters.Add(
                    "@LastName",
                    SqlDbType.NVarChar,
                    100).Value =
                    ToDbValue(lastName);

                command.Parameters.Add(
                    "@Email",
                    SqlDbType.NVarChar,
                    150).Value =
                    ToDbValue(
                        Truncate(
                            data.Email,
                            150));

                command.Parameters.Add(
                    "@Phone",
                    SqlDbType.NVarChar,
                    20).Value =
                    data.Mobile;

                command.Parameters.Add(
                    "@FullName",
                    SqlDbType.NVarChar,
                    150).Value =
                    data.FullName;

                command.Parameters.Add(
                    "@UserId",
                    SqlDbType.Int).Value =
                    userId;

                int affected =
                    command.ExecuteNonQuery();

                if (affected != 1)
                {
                    throw new ApplicationException(
                        "The donor user account could not be updated.");
                }
            }
        }

        // =========================================================
        // INSERT DONOR
        // =========================================================

        private int InsertDonor(
            SqlConnection connection,
            SqlTransaction transaction,
            int userId,
            DonorFormData data)
        {
            const string sql = @"
                INSERT INTO dbo.Donors
                (
                    FullName,
                    BloodGroup,
                    Mobile,
                    City,
                    State,
                    IsAvailable,
                    CreatedDate,
                    UserId,
                    Email,
                    Address,
                    Pincode,
                    UpdatedDate,
                    Gender,
                    DateOfBirth
                )
                OUTPUT INSERTED.DonorId
                VALUES
                (
                    @FullName,
                    @BloodGroup,
                    @Mobile,
                    @City,
                    @State,
                    @IsAvailable,
                    GETDATE(),
                    @UserId,
                    @Email,
                    @Address,
                    @Pincode,
                    GETDATE(),
                    @Gender,
                    @DateOfBirth
                );";

            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection,
                    transaction))
            {
                AddDonorParameters(
                    command,
                    userId,
                    data);

                object result =
                    command.ExecuteScalar();

                if (result == null ||
                    result == DBNull.Value)
                {
                    throw new ApplicationException(
                        "Unable to create the donor record.");
                }

                return Convert.ToInt32(result);
            }
        }

        // =========================================================
        // ADMIN UPDATE DONOR
        // =========================================================

        private void UpdateDonorAsAdmin(
            SqlConnection connection,
            SqlTransaction transaction,
            int donorId,
            int userId,
            DonorFormData data)
        {
            const string sql = @"
                UPDATE dbo.Donors
                SET
                    FullName = @FullName,
                    BloodGroup = @BloodGroup,
                    Mobile = @Mobile,
                    City = @City,
                    State = @State,
                    Email = @Email,
                    Address = @Address,
                    Pincode = @Pincode,
                    UpdatedDate = GETDATE(),
                    Gender = @Gender,
                    DateOfBirth = @DateOfBirth
                WHERE DonorId = @DonorId
                  AND UserId = @UserId;";

            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection,
                    transaction))
            {
                // IMPORTANT:
                //
                // There is NO IsAvailable field here.
                //
                // Therefore admin edit cannot change availability.

                AddDonorParametersWithoutAvailability(
                    command,
                    userId,
                    data);

                command.Parameters.Add(
                    "@DonorId",
                    SqlDbType.Int).Value =
                    donorId;

                int affected =
                    command.ExecuteNonQuery();

                if (affected != 1)
                {
                    throw new ApplicationException(
                        "The donor record could not be updated.");
                }
            }
        }

        // =========================================================
        // OWNER UPDATE DONOR
        // =========================================================

        private void UpdateDonorForOwner(
            SqlConnection connection,
            SqlTransaction transaction,
            int donorId,
            int userId,
            DonorFormData data)
        {
            const string sql = @"
                UPDATE dbo.Donors
                SET
                    FullName = @FullName,
                    BloodGroup = @BloodGroup,
                    Mobile = @Mobile,
                    City = @City,
                    State = @State,
                    IsAvailable = @IsAvailable,
                    Email = @Email,
                    Address = @Address,
                    Pincode = @Pincode,
                    UpdatedDate = GETDATE(),
                    Gender = @Gender,
                    DateOfBirth = @DateOfBirth
                WHERE DonorId = @DonorId
                  AND UserId = @UserId;";

            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection,
                    transaction))
            {
                // Owner is allowed to change availability.

                AddDonorParameters(
                    command,
                    userId,
                    data);

                command.Parameters.Add(
                    "@DonorId",
                    SqlDbType.Int).Value =
                    donorId;

                int affected =
                    command.ExecuteNonQuery();

                if (affected != 1)
                {
                    throw new ApplicationException(
                        "The donor record could not be updated.");
                }
            }
        }

        // =========================================================
        // DONOR PARAMETERS WITH AVAILABILITY
        // =========================================================

        private void AddDonorParameters(
            SqlCommand command,
            int userId,
            DonorFormData data)
        {
            command.Parameters.Add(
                "@FullName",
                SqlDbType.NVarChar,
                150).Value =
                data.FullName;

            command.Parameters.Add(
                "@BloodGroup",
                SqlDbType.NVarChar,
                5).Value =
                data.BloodGroup;

            command.Parameters.Add(
                "@Mobile",
                SqlDbType.NVarChar,
                20).Value =
                data.Mobile;

            command.Parameters.Add(
                "@City",
                SqlDbType.NVarChar,
                100).Value =
                data.City;

            command.Parameters.Add(
                "@State",
                SqlDbType.NVarChar,
                100).Value =
                ToDbValue(data.State);

            command.Parameters.Add(
                "@IsAvailable",
                SqlDbType.Bit).Value =
                data.IsAvailable;

            command.Parameters.Add(
                "@UserId",
                SqlDbType.Int).Value =
                userId;

            command.Parameters.Add(
                "@Email",
                SqlDbType.NVarChar,
                255).Value =
                ToDbValue(data.Email);

            command.Parameters.Add(
                "@Address",
                SqlDbType.NVarChar,
                500).Value =
                ToDbValue(data.Address);

            command.Parameters.Add(
                "@Pincode",
                SqlDbType.NVarChar,
                10).Value =
                ToDbValue(data.Pincode);

            command.Parameters.Add(
                "@Gender",
                SqlDbType.NVarChar,
                20).Value =
                ToDbValue(data.Gender);

            command.Parameters.Add(
                "@DateOfBirth",
                SqlDbType.Date).Value =
                data.DateOfBirth.Date;
        }

        // =========================================================
        // DONOR PARAMETERS WITHOUT AVAILABILITY
        // =========================================================

        private void AddDonorParametersWithoutAvailability(
            SqlCommand command,
            int userId,
            DonorFormData data)
        {
            command.Parameters.Add(
                "@FullName",
                SqlDbType.NVarChar,
                150).Value =
                data.FullName;

            command.Parameters.Add(
                "@BloodGroup",
                SqlDbType.NVarChar,
                5).Value =
                data.BloodGroup;

            command.Parameters.Add(
                "@Mobile",
                SqlDbType.NVarChar,
                20).Value =
                data.Mobile;

            command.Parameters.Add(
                "@City",
                SqlDbType.NVarChar,
                100).Value =
                data.City;

            command.Parameters.Add(
                "@State",
                SqlDbType.NVarChar,
                100).Value =
                ToDbValue(data.State);

            // IMPORTANT:
            //
            // No @IsAvailable parameter.
            //
            // Admin edit cannot modify availability.

            command.Parameters.Add(
                "@UserId",
                SqlDbType.Int).Value =
                userId;

            command.Parameters.Add(
                "@Email",
                SqlDbType.NVarChar,
                255).Value =
                ToDbValue(data.Email);

            command.Parameters.Add(
                "@Address",
                SqlDbType.NVarChar,
                500).Value =
                ToDbValue(data.Address);

            command.Parameters.Add(
                "@Pincode",
                SqlDbType.NVarChar,
                10).Value =
                ToDbValue(data.Pincode);

            command.Parameters.Add(
                "@Gender",
                SqlDbType.NVarChar,
                20).Value =
                ToDbValue(data.Gender);

            command.Parameters.Add(
                "@DateOfBirth",
                SqlDbType.Date).Value =
                data.DateOfBirth.Date;
        }

        // =========================================================
        // DONOR ROLE
        // =========================================================

        private int GetDonorRoleId(
            SqlConnection connection,
            SqlTransaction transaction)
        {
            const string sql = @"
                SELECT TOP 1 RoleId
                FROM dbo.Roles
                WHERE RoleName = 'Donor';";

            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection,
                    transaction))
            {
                object result =
                    command.ExecuteScalar();

                if (result == null ||
                    result == DBNull.Value)
                {
                    return 0;
                }

                return Convert.ToInt32(result);
            }
        }

        // =========================================================
        // LOAD DONOR BY DONOR ID
        // =========================================================

        private DonorData LoadDonorById(
            int donorId)
        {
            if (donorId <= 0)
            {
                return null;
            }

            const string sql = @"
                SELECT
                    DonorId,
                    FullName,
                    BloodGroup,
                    Mobile,
                    City,
                    State,
                    IsAvailable,
                    UserId,
                    Email,
                    Address,
                    Pincode,
                    Gender,
                    DateOfBirth
                FROM dbo.Donors
                WHERE DonorId = @DonorId;";

            using (SqlConnection connection =
                Db.OpenConnection())
            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection))
            {
                command.Parameters.Add(
                    "@DonorId",
                    SqlDbType.Int).Value =
                    donorId;

                using (SqlDataReader reader =
                    command.ExecuteReader())
                {
                    return reader.Read()
                        ? ReadDonor(reader)
                        : null;
                }
            }
        }

        // =========================================================
        // LOAD DONOR BY USER ID
        // =========================================================

        private DonorData LoadDonorByUserId(
            int userId)
        {
            if (userId <= 0)
            {
                return null;
            }

            const string sql = @"
                SELECT
                    DonorId,
                    FullName,
                    BloodGroup,
                    Mobile,
                    City,
                    State,
                    IsAvailable,
                    UserId,
                    Email,
                    Address,
                    Pincode,
                    Gender,
                    DateOfBirth
                FROM dbo.Donors
                WHERE UserId = @UserId;";

            using (SqlConnection connection =
                Db.OpenConnection())
            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection))
            {
                command.Parameters.Add(
                    "@UserId",
                    SqlDbType.Int).Value =
                    userId;

                using (SqlDataReader reader =
                    command.ExecuteReader())
                {
                    return reader.Read()
                        ? ReadDonor(reader)
                        : null;
                }
            }
        }

        // =========================================================
        // READ DONOR
        // =========================================================

        private DonorData ReadDonor(
            SqlDataReader reader)
        {
            return new DonorData
            {
                DonorId =
                    GetInt32(
                        reader,
                        "DonorId"),

                FullName =
                    GetString(
                        reader,
                        "FullName"),

                BloodGroup =
                    GetString(
                        reader,
                        "BloodGroup"),

                Mobile =
                    GetString(
                        reader,
                        "Mobile"),

                City =
                    GetString(
                        reader,
                        "City"),

                State =
                    GetString(
                        reader,
                        "State"),

                IsAvailable =
                    GetBoolean(
                        reader,
                        "IsAvailable"),

                UserId =
                    GetInt32(
                        reader,
                        "UserId"),

                Email =
                    GetString(
                        reader,
                        "Email"),

                Address =
                    GetString(
                        reader,
                        "Address"),

                Pincode =
                    GetString(
                        reader,
                        "Pincode"),

                Gender =
                    GetString(
                        reader,
                        "Gender"),

                DateOfBirth =
                    GetNullableDateTime(
                        reader,
                        "DateOfBirth")
            };
        }

        // =========================================================
        // LOAD DONOR INTO FORM
        // =========================================================

        private void LoadDonorIntoForm(
            DonorData donor)
        {
            txtFullName.Text =
                donor.FullName;

            SetDropDownValue(
                ddlBloodGroup,
                donor.BloodGroup);

            SetDropDownValue(
                ddlGender,
                donor.Gender);

            txtMobile.Text =
                donor.Mobile;

            txtEmail.Text =
                donor.Email;

            txtAddress.Text =
                donor.Address;

            txtCity.Text =
                donor.City;

            txtState.Text =
                donor.State;

            txtPincode.Text =
                donor.Pincode;

            chkAvailable.Checked =
                donor.IsAvailable;

            txtDateOfBirth.Text =
                donor.DateOfBirth.HasValue
                    ? donor.DateOfBirth.Value.ToString(
                        "yyyy-MM-dd",
                        CultureInfo.InvariantCulture)
                    : string.Empty;
        }

        // =========================================================
        // DATE OF BIRTH
        // =========================================================

        protected void ValidateDateOfBirth(
            object source,
            ServerValidateEventArgs args)
        {
            DateTime dateOfBirth;

            if (!TryParseDateOfBirth(
                args.Value,
                out dateOfBirth))
            {
                args.IsValid = false;
                return;
            }

            args.IsValid =
                IsValidAge(dateOfBirth);
        }

        private bool TryGetDateOfBirth(
            out DateTime dateOfBirth)
        {
            return TryParseDateOfBirth(
                txtDateOfBirth.Text,
                out dateOfBirth);
        }

        private bool TryParseDateOfBirth(
            string value,
            out DateTime dateOfBirth)
        {
            return DateTime.TryParseExact(
                value == null
                    ? string.Empty
                    : value.Trim(),
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out dateOfBirth);
        }

        private bool IsValidAge(
            DateTime dateOfBirth)
        {
            DateTime today =
                DateTime.Today;

            if (dateOfBirth.Date > today)
            {
                return false;
            }

            int age =
                today.Year -
                dateOfBirth.Year;

            if (dateOfBirth.Date >
                today.AddYears(-age))
            {
                age--;
            }

            return age >= MinimumDonorAge &&
                   age <= MaximumDonorAge;
        }

        // =========================================================
        // BUSINESS VALIDATION
        // =========================================================

        private bool ValidateBusinessData(
            DonorFormData data)
        {
            if (string.IsNullOrWhiteSpace(
                data.FullName))
            {
                ShowError(
                    "Full name is required.");

                return false;
            }

            if (data.FullName.Length > 150)
            {
                ShowError(
                    "Full name cannot exceed 150 characters.");

                return false;
            }

            if (!IsValidBloodGroup(
                data.BloodGroup))
            {
                ShowError(
                    "Please select a valid blood group.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                data.Mobile))
            {
                ShowError(
                    "Mobile number is required.");

                return false;
            }

            if (!IsValidIndianMobile(
                data.Mobile))
            {
                ShowError(
                    "Please enter a valid 10-digit mobile number.");

                return false;
            }

            if (string.IsNullOrWhiteSpace(
                data.City))
            {
                ShowError(
                    "City is required.");

                return false;
            }

            if (!IsValidAge(
                data.DateOfBirth))
            {
                ShowError(
                    "Donor age must be between 18 and 65 years.");

                return false;
            }

            if (!string.IsNullOrWhiteSpace(
                data.Pincode) &&
                !IsValidPincode(
                    data.Pincode))
            {
                ShowError(
                    "Please enter a valid 6-digit pincode.");

                return false;
            }

            return true;
        }

        private bool IsValidBloodGroup(
            string bloodGroup)
        {
            switch (bloodGroup)
            {
                case "A+":
                case "A-":
                case "B+":
                case "B-":
                case "AB+":
                case "AB-":
                case "O+":
                case "O-":
                    return true;

                default:
                    return false;
            }
        }

        private bool IsValidIndianMobile(
            string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile) ||
                mobile.Length != 10)
            {
                return false;
            }

            if (mobile[0] < '6' ||
                mobile[0] > '9')
            {
                return false;
            }

            for (int i = 1;
                 i < mobile.Length;
                 i++)
            {
                if (!char.IsDigit(mobile[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsValidPincode(
            string pincode)
        {
            if (pincode.Length != 6 ||
                pincode[0] == '0')
            {
                return false;
            }

            for (int i = 0;
                 i < pincode.Length;
                 i++)
            {
                if (!char.IsDigit(pincode[i]))
                {
                    return false;
                }
            }

            return true;
        }

        // =========================================================
        // USERNAME GENERATION
        // =========================================================

        private string GenerateUniqueUsername(string firstName,string lastName,string mobile)
        {
            string first =
                RemoveInvalidUsernameCharacters(
                    firstName);

            string last =
                RemoveInvalidUsernameCharacters(
                    lastName);

            string baseUsername =
                string.IsNullOrWhiteSpace(last)
                    ? first
                    : first + "." + last;

            if (string.IsNullOrWhiteSpace(
                baseUsername))
            {
                baseUsername = "donor";
            }

            baseUsername =
                baseUsername.ToLowerInvariant();

            // =========================================================
            // FIRST USER
            // =========================================================
            //
            // Example:
            // john.smith
            //
            // If this username is available, use it directly.
            // =========================================================

            baseUsername =
                Truncate(
                    baseUsername,
                    100);

            if (!UsernameExists(baseUsername))
            {
                return baseUsername;
            }

            // =========================================================
            // DUPLICATE USERS
            // =========================================================
            //
            // Example:
            //
            // john.smith
            // john.smith01
            // john.smith02
            // john.smith03
            // ...
            //
            // Always use a two-digit suffix.
            // =========================================================

            for (int counter = 1;
                 counter <= 9999;
                 counter++)
            {
                string counterText =
                    counter.ToString(
                        "00",
                        CultureInfo.InvariantCulture);

                int maximumBaseLength =
                    100 -
                    counterText.Length;

                string username =
                    Truncate(
                        baseUsername,
                        maximumBaseLength) +
                    counterText;

                if (!UsernameExists(username))
                {
                    return username;
                }
            }

            throw new ApplicationException(
                "Unable to generate a unique username.");
        }

        private bool UsernameExists(
            string username)
        {
            const string sql = @"
                SELECT COUNT(1)
                FROM dbo.Users
                WHERE Username = @Username;";

            using (SqlConnection connection =
                Db.OpenConnection())
            using (SqlCommand command =
                new SqlCommand(
                    sql,
                    connection))
            {
                command.Parameters.Add(
                    "@Username",
                    SqlDbType.NVarChar,
                    100).Value =
                    username;

                return Convert.ToInt32(
                    command.ExecuteScalar()) > 0;
            }
        }

        // =========================================================
        // NAME HELPERS
        // =========================================================

        private void SplitName(
            string fullName,
            out string firstName,
            out string lastName)
        {
            string[] parts =
                (fullName ?? string.Empty)
                    .Trim()
                    .Split(
                        new[] { ' ' },
                        StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length == 0)
            {
                firstName = "Donor";
                lastName = string.Empty;
                return;
            }

            firstName =
                parts[0];

            if (parts.Length == 1)
            {
                lastName =
                    string.Empty;

                return;
            }

            StringBuilder builder =
                new StringBuilder();

            for (int i = 1;
                 i < parts.Length;
                 i++)
            {
                if (builder.Length > 0)
                {
                    builder.Append(" ");
                }

                builder.Append(parts[i]);
            }

            lastName =
                builder.ToString();
        }

        private string RemoveInvalidUsernameCharacters(
            string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            StringBuilder builder =
                new StringBuilder();

            foreach (char character in value)
            {
                if (char.IsLetterOrDigit(character))
                {
                    builder.Append(character);
                }
            }

            return builder.ToString();
        }

        // =========================================================
        // FORM HELPERS
        // =========================================================

        private void ClearForm()
        {
            txtFullName.Text =
                string.Empty;

            ddlBloodGroup.SelectedIndex =
                0;

            ddlGender.SelectedIndex =
                0;

            txtDateOfBirth.Text =
                string.Empty;

            txtMobile.Text =
                string.Empty;

            txtEmail.Text =
                string.Empty;

            txtAddress.Text =
                string.Empty;

            txtCity.Text =
                string.Empty;

            txtState.Text =
                string.Empty;

            txtPincode.Text =
                string.Empty;

            // New registration is unavailable.
            chkAvailable.Checked =
                false;
        }

        private void DisableForm()
        {
            txtFullName.Enabled = false;
            ddlBloodGroup.Enabled = false;
            ddlGender.Enabled = false;
            txtDateOfBirth.Enabled = false;
            txtMobile.Enabled = false;
            txtEmail.Enabled = false;
            txtAddress.Enabled = false;
            txtCity.Enabled = false;
            txtState.Enabled = false;
            txtPincode.Enabled = false;
            chkAvailable.Enabled = false;
            btnSave.Enabled = false;
        }

        private void SetDropDownValue(
            DropDownList list,
            string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                list.SelectedIndex = 0;
                return;
            }

            ListItem item =
                list.Items.FindByValue(value);

            list.SelectedValue =
                item == null
                    ? string.Empty
                    : value;
        }

        // =========================================================
        // MESSAGES
        // =========================================================

        private void HideMessages()
        {
            pnlMessage.Visible =
                false;
        }

        private void ShowSuccess(
     string message)
        {
            pnlMessage.Visible =
                true;

            pnlMessage.CssClass =
                "message message-success";

            litMessage.Text =
                Server.HtmlEncode(
                    message);

            // =====================================================
            // SUCCESS MESSAGE:
            // Display for exactly 2 seconds.
            // Then hide ONLY the success message.
            //
            // This does NOT redirect the page.
            // Donor credentials remain visible for their full
            // 60-second countdown.
            // =====================================================

            string script =
                "(function () {" +

                "var message = " +
                "document.getElementById('" +
                pnlMessage.ClientID +
                "');" +

                "if (message) {" +

                "setTimeout(function () {" +
                "message.style.display = 'none';" +
                "}, 2000);" +

                "}" +

                "})();";

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "HideSuccessMessage",
                script,
                true);
        }

        private void ShowError(
            string message)
        {
            pnlMessage.Visible =
                true;

            pnlMessage.CssClass =
                "message message-error";

            litMessage.Text =
                Server.HtmlEncode(
                    message);
        }

        // =========================================================
        // GENERATED CREDENTIALS
        // =========================================================

        private void ShowGeneratedCredentials(
    string donorName,
    string username,
    string temporaryPassword)
        {
            pnlCredentials.Visible = true;

            litCredentialDonorName.Text =
                Server.HtmlEncode(
                    donorName);

            litCredentialUsername.Text =
                Server.HtmlEncode(
                    username);

            litCredentialPassword.Text =
                Server.HtmlEncode(
                    temporaryPassword);

            // =====================================================
            // CREDENTIALS:
            // Display for exactly 60 seconds.
            // Then redirect to Dashboard.
            //
            // The success message has its own separate
            // 2-second hide timer in ShowSuccess().
            // =====================================================

            string redirectUrl =
                ResolveUrl("~/Dashboard.aspx")
                    .Replace("\\", "\\\\")
                    .Replace("'", "\\'");

            string script =
                "(function () {" +

                "var remaining = 60;" +

                "var countdown = " +
                "document.getElementById('credentialCountdown');" +

                "if (countdown) {" +
                "countdown.innerHTML = " +
                "'Redirecting in ' + remaining + ' seconds...';" +
                "}" +

                "var timer = setInterval(function () {" +

                "remaining--;" +

                "if (countdown) {" +
                "countdown.innerHTML = " +
                "'Redirecting in ' + remaining + ' seconds...';" +
                "}" +

                "if (remaining <= 0) {" +

                "clearInterval(timer);" +

                "window.location.href='" +
                redirectUrl +
                "';" +

                "}" +

                "}, 1000);" +

                "})();";

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "CredentialCountdown",
                script,
                true);
        }

        // =========================================================
        // DELAYED REDIRECT
        // =========================================================

        private void RedirectAfterDelay(
            string url,
            int delayMilliseconds)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            if (delayMilliseconds < 0)
            {
                delayMilliseconds = 0;
            }

            string encodedUrl =
                ResolveUrl(url)
                    .Replace("\\", "\\\\")
                    .Replace("'", "\\'");

            string script =
                "setTimeout(function () {" +
                "window.location.href='" +
                encodedUrl +
                "';" +
                "}, " +
                delayMilliseconds.ToString(
                    CultureInfo.InvariantCulture) +
                ");";

            ScriptManager.RegisterStartupScript(
                this,
                GetType(),
                "DelayedRedirect_" +
                Guid.NewGuid().ToString("N"),
                script,
                true);
        }

        // =========================================================
        // DATABASE ERROR HANDLING
        // =========================================================

        private void HandleDatabaseException(
            SqlException exception,
            string defaultMessage)
        {
            if (exception.Number == 2601 ||
                exception.Number == 2627)
            {
                ShowError(
                    "A donor or user with the same unique information already exists.");

                return;
            }

            ShowError(
                defaultMessage);
        }

        private void SafeRollback(
            SqlTransaction transaction)
        {
            try
            {
                if (transaction != null)
                {
                    transaction.Rollback();
                }
            }
            catch
            {
                // Preserve original exception.
            }
        }

        // =========================================================
        // DATABASE VALUE HELPERS
        // =========================================================

        private int GetInt32(
            SqlDataReader reader,
            string column)
        {
            if (reader[column] ==
                DBNull.Value)
            {
                return 0;
            }

            return Convert.ToInt32(
                reader[column]);
        }

        private string GetString(
            SqlDataReader reader,
            string column)
        {
            if (reader[column] ==
                DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToString(
                reader[column]);
        }

        private bool GetBoolean(
            SqlDataReader reader,
            string column)
        {
            if (reader[column] ==
                DBNull.Value)
            {
                return false;
            }

            return Convert.ToBoolean(
                reader[column]);
        }

        private DateTime? GetNullableDateTime(
            SqlDataReader reader,
            string column)
        {
            if (reader[column] ==
                DBNull.Value)
            {
                return null;
            }

            return Convert.ToDateTime(
                reader[column]);
        }

        // =========================================================
        // TEXT HELPERS
        // =========================================================

        private string CleanText(
            string value,
            int maxLength)
        {
            if (value == null)
            {
                return string.Empty;
            }

            return Truncate(
                value.Trim(),
                maxLength);
        }

        private string CleanNullableText(
            string value,
            int maxLength)
        {
            string cleaned =
                CleanText(
                    value,
                    maxLength);

            return string.IsNullOrWhiteSpace(
                cleaned)
                ? null
                : cleaned;
        }

        private string Truncate(
            string value,
            int maxLength)
        {
            if (string.IsNullOrEmpty(value) ||
                value.Length <= maxLength)
            {
                return value;
            }

            return value.Substring(
                0,
                maxLength);
        }

        private object ToDbValue(
            string value)
        {
            return string.IsNullOrWhiteSpace(
                value)
                ? (object)DBNull.Value
                : value;
        }

        // =========================================================
        // DATA CLASSES
        // =========================================================

        private class DonorFormData
        {
            public string FullName { get; set; }

            public string BloodGroup { get; set; }

            public string Gender { get; set; }

            public DateTime DateOfBirth { get; set; }

            public string Mobile { get; set; }

            public string Email { get; set; }

            public string Address { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string Pincode { get; set; }

            public bool IsAvailable { get; set; }
        }

        private class DonorData
        {
            public int DonorId { get; set; }

            public string FullName { get; set; }

            public string BloodGroup { get; set; }

            public string Mobile { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public bool IsAvailable { get; set; }

            public int UserId { get; set; }

            public string Email { get; set; }

            public string Address { get; set; }

            public string Pincode { get; set; }

            public string Gender { get; set; }

            public DateTime? DateOfBirth { get; set; }
        }
    }
}

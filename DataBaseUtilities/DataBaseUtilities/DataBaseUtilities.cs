using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using ProjectType;
using CAccounts;

namespace DataBaseUtilities
{
    public enum EventFileType
    {
        Input = 1, Output = 2
    }

    /// <summary>
    /// Будь-який метод класу може згенерувати виключення класу SqlException, ті виключення які я генерую сам зазначені в описі методу
    /// </summary>
    public static class DbUtilities
    {
        private static readonly string ConnectionString = ConfigurationManager.ConnectionStrings["Development"].ConnectionString;
        public delegate void FileHandler(int eventId);
        public static event FileHandler OnEventDelete;

        #region general staff for dealing with accounts

        /// <summary>
        ///     Додає переданий аккаунт в базу
        ///     
        /// </summary>
        /// <param name="account">Об'єкт, який необхідно додати</param>
        /// <exception cref="ArgumentException">Якщо не визначено тип аккаунту</exception>
        public static void AddAccount(Account account)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand query;
                switch (account.AccountType)
                {
                    case AccountType.Student:
                        Student student = (Student)account;
                        query = new SqlCommand("SELECT COUNT(StudentId) FROM Student WHERE RBNumber = @RBNumber", connection);
                        query.Parameters.Add("@RBNumber", SqlDbType.VarChar, 10).Value = student.RBNumber;
                        if ((int)query.ExecuteScalar() > 0)
                            throw new ArgumentException("Студент з даним номер залікової книжки вже існує");
                        int groupId = GetGroupIdByName(student.Group);
                        if (groupId < 0)
                            throw new ArgumentOutOfRangeException("account", "група не знайдена");
                        query = new SqlCommand("INSERT INTO Student(AccountId, RBNumber, SpecialityCode, GroupId, FacultyId, EFormId, ELevelId, CountA, CountB, CountC, CountD, CountE) VALUES(@AccountId, @RBNumber, @SpecialityCode, @GroupId, @FacultyId, @EFormId, @ELevelId, @CountA, @CountB, @CountC, @CountD, @CountE); SELECT CAST(scope_identity() AS int)", connection);
                        query.Parameters.Add("@RBNumber", SqlDbType.VarChar, 10).Value = student.RBNumber;
                        query.Parameters.Add("@SpecialityCode", SqlDbType.VarChar, 10).Value = student.SpecialityCode;
                        query.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupId;
                        query.Parameters.Add("@FacultyId", SqlDbType.Int).Value = student.Faculty;
                        query.Parameters.Add("@EFormId", SqlDbType.Int).Value = student.EducationForm;
                        query.Parameters.Add("@ELevelId", SqlDbType.Int).Value = student.EducationLevel;
                        query.Parameters.Add("@CountA", SqlDbType.Int).Value = student.Acount;
                        query.Parameters.Add("@CountB", SqlDbType.Int).Value = student.Bcount;
                        query.Parameters.Add("@CountC", SqlDbType.Int).Value = student.Ccount;
                        query.Parameters.Add("@CountD", SqlDbType.Int).Value = student.Dcount;
                        query.Parameters.Add("@CountE", SqlDbType.Int).Value = student.Ecount;
                        break;
                    case AccountType.Instructor:
                        Instructor instructor = (Instructor)account;
                        query = new SqlCommand("INSERT INTO Instructor(AccountId, Position, AcademicStatus, AcademicLevel, WorkPlace) VALUES(@AccountId, @Position, @AcademicStatus, @AcademicLevel, @WorkPlace); SELECT CAST(scope_identity() AS int)", connection);
                        query.Parameters.Add("@Position", SqlDbType.VarChar, 100).Value = instructor.Position;
                        query.Parameters.Add("@AcademicStatus", SqlDbType.VarChar, 100).Value = instructor.AcademicStatus;
                        query.Parameters.Add("@AcademicLevel", SqlDbType.VarChar, 100).Value = instructor.AcademicLevel;
                        query.Parameters.Add("@WorkPlace", SqlDbType.VarChar, 100).Value = instructor.Workplace;
                        break;
                    case AccountType.Normokontroler:
                        Normokontroler normokontroler = (Normokontroler)account;
                        query = new SqlCommand("INSERT INTO Normokontroler(AccountId, Position, WorkPlace) VALUES(@AccountId, @Position, @WorkPlace); SELECT CAST(scope_identity() AS int)", connection);
                        query.Parameters.Add("@Position", SqlDbType.VarChar, 100).Value = normokontroler.Position;
                        query.Parameters.Add("@WorkPlace", SqlDbType.VarChar, 100).Value = normokontroler.Workplace;
                        break;
                    default:
                        throw new ArgumentException("Невірний тип аккаунту", "account");
                }

                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand insertBase = new SqlCommand("INSERT INTO Account(Login, Password, AccountType, FirstName, LastName, Patronymic, Phone, Address, Email) VALUES(@Login, @Password, @AccountType, @FirstName, @LastName, @Patronymic, @Phone, @Address, @Email); SELECT CAST(scope_identity() AS int)", connection, transaction))
                {
                    try
                    {
                        insertBase.Parameters.Add("@Login", SqlDbType.VarChar, 30).Value = account.Login;
                        insertBase.Parameters.Add("@AccountType", SqlDbType.Int).Value = (int)account.AccountType;
                        insertBase.Parameters.Add("@Password", SqlDbType.VarChar, 30).Value = account.Password;
                        insertBase.Parameters.Add("@FirstName", SqlDbType.VarChar, 30).Value = account.Firstname;
                        insertBase.Parameters.Add("@LastName", SqlDbType.VarChar, 30).Value = account.Lastname;
                        insertBase.Parameters.Add("@Patronymic", SqlDbType.VarChar, 30).Value = account.Patronymic;
                        insertBase.Parameters.Add("@Phone", SqlDbType.VarChar, 20).Value = account.PhoneNumber;
                        insertBase.Parameters.Add("@Address", SqlDbType.VarChar, 100).Value = account.Address;
                        insertBase.Parameters.Add("@Email", SqlDbType.VarChar, 30).Value = account.Email;
                        account.ID = (int)insertBase.ExecuteScalar();
                        query.Parameters.Add("@AccountId", SqlDbType.Int).Value = account.ID;
                        query.Transaction = transaction;
                        int subId = (int)query.ExecuteScalar();
                        switch (account.AccountType)
                        {
                            case AccountType.Student:
                                ((Student)account).StudentId = subId;
                                break;
                            case AccountType.Instructor:
                                ((Instructor)account).InstructorId = subId;
                                break;
                            case AccountType.Normokontroler:
                                ((Normokontroler)account).NormokontrolerId = subId;
                                break;
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
                query.Dispose();
            }

        }

        /// <summary>Отримання типу користувача в залежності від логіну та пароля</summary>
        /// <param name="login">Логін користувача</param>
        /// <param name="password">Пароль користувача</param>
        /// <returns>
        ///     0 - якщо передана пара логін/пароль не знайдено
        ///     1 - якщо тип користувача студент
        ///     2 - якщо тип користувача викладач
        ///     3 - якщо тип користувача нормоконтроль
        /// </returns>
        public static AccountType CheckLogin(string login, string password)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    using (SqlCommand query = new SqlCommand("SELECT AccountType FROM Account WHERE Login = @Login AND Password = @Password", connection))
                    {
                        query.Parameters.Add("@Login", SqlDbType.VarChar, 30).Value = login;
                        query.Parameters.Add("@Password", SqlDbType.VarChar, 30).Value = password;
                        return (AccountType)(query.ExecuteScalar() ?? 0);
                    }
                }

            }
            catch (SqlException ex)
            {
                File.AppendAllText("sql_error.log", ex.ToString());
                throw;
            }
        }

        /// <summary>
        ///     Вхід користувача в систему
        /// </summary>
        /// <param name="login"></param>
        /// <param name="password"></param>
        /// <returns>При співпадінні логіну й пароля повертає відповідний об'єкт облікового запису</returns>
        public static Account Login(string login, string password)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand query = new SqlCommand(@"SELECT *  
FROM Account 
LEFT JOIN Student on (AccountType = 1 AND Account.AccountId = Student.AccountId)
LEFT JOIN Instructor on (AccountType = 2 AND Account.AccountId = Instructor.AccountId) 
LEFT JOIN Normokontroler on (AccountType = 3 AND Account.AccountId = Normokontroler.AccountId)
WHERE Login = @Login AND Password = @Password", connection))
            {
                connection.Open();
                query.Parameters.Add("@Login", SqlDbType.VarChar, 30).Value = login;
                query.Parameters.Add("@Password", SqlDbType.VarChar, 30).Value = password;
                using (SqlDataReader reader = query.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    reader.Read();
                    return CreateAccountObject(reader, (AccountType)reader["AccountType"]);
                }
            }
        }

        /// <summary>
        ///     Отримуємо об'єкт заданого типу по id аккаунту
        /// </summary>
        /// <param name="accountId">id аккаунту</param>
        /// <returns>Об'єкт аккаунти чи null якщо не знайдений</returns>
        public static Account GetAccountbyId(int accountId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand query = new SqlCommand(string.Format("SELECT * FROM Account LEFT JOIN Student on (Account.AccountId = Student.AccountId AND Account.AccountType = 1) LEFT JOIN Instructor on (Account.AccountId = Instructor.AccountId AND Account.AccountType = 2) LEFT JOIN Normokontroler on (Account.AccountId = Normokontroler.AccountId AND Account.AccountType = 3) WHERE Account.AccountId = @Id"), connection))
                {
                    query.Parameters.Add("@Id", SqlDbType.Int).Value = accountId;
                    using (SqlDataReader reader = query.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return null;
                        reader.Read();
                        return CreateAccountObject(reader, (AccountType)reader["AccountType"]);
                    }
                }
            }
        }

        /// <summary>
        ///     Допоміжний метод створює об'єкт відповідного типу аккаунта
        /// </summary>
        /// <param name="reader">Дані отримані з бази(здійснює перевірку на наявність записів)</param>
        /// <param name="type">Тип аккаунту який потрібно створити</param>
        /// <returns>Account відповідного типу</returns>
        /// <exception cref="ArgumentException">Якщо тип аккаунту задано не вірно</exception>
        private static Account CreateAccountObject(SqlDataReader reader, AccountType type)
        {
            if (!reader.HasRows)
                return null;
            Account account;
            switch (type)
            {
                case AccountType.Student:
                    string groupName = GetGroupById((int)reader["GroupId"]);
                    account = new Student((string)reader["Login"], (string)reader["Password"],
                        (string)reader["FirstName"], (string)reader["LastName"],
                        (string)reader["Patronymic"], (string)reader["RBNumber"], (string)reader["Address"], (string)reader["Phone"], (string)reader["Email"])
                    {
                        ID = (int)reader["AccountId"],
                        StudentId = (int)reader["StudentId"],
                        //Поля типу студент
                        SpecialityCode = (string)reader["SpecialityCode"],
                        Group = groupName,
                        Faculty = (Faculties)reader["FacultyId"],
                        EducationForm = (EducationForms)reader["EFormId"],
                        EducationLevel = (EducationLevels)reader["ELevelId"],
                        Acount = (int)reader["CountA"],
                        Bcount = (int)reader["CountB"],
                        Ccount = (int)reader["CountC"],
                        Dcount = (int)reader["CountD"],
                        Ecount = (int)reader["CountE"]
                    };
                    break;
                case AccountType.Instructor:
                    account = new Instructor((string)reader["Login"], (string)reader["Password"],
                    (string)reader["FirstName"], (string)reader["LastName"],
                    (string)reader["Patronymic"], (string)reader["Address"], (string)reader["Phone"], (string)reader["Email"])
                    {
                        ID = (int)reader["AccountId"],
                        InstructorId = (int)reader["InstructorId"],
                        //Поля викладача
                        Position = (string)(reader["Position"] is DBNull ? "" : reader["Position"]),
                        Workplace = (string)(reader["WorkPlace"] is DBNull ? "" : reader["WorkPlace"]),
                        AcademicLevel = (string)(reader["AcademicLevel"] is DBNull ? "" : reader["AcademicLevel"]),
                        AcademicStatus = (string)(reader["AcademicStatus"] is DBNull ? "" : reader["AcademicStatus"]),

                    };
                    break;
                case AccountType.Normokontroler:
                    account = new Normokontroler((string)reader["Login"], (string)reader["Password"],
                        (string)reader["FirstName"], (string)reader["LastName"],
                        (string)reader["Patronymic"], (string)reader["Address"], (string)reader["Phone"], (string)reader["Email"])
                    {
                        ID = (int)reader["AccountId"],
                        NormokontrolerId = (int)reader["NormokontrolerId"],
                        Position = (string)(reader["Position"] is DBNull ? "" : reader.GetValue(32)),
                        Workplace = (string)(reader["WorkPlace"] is DBNull ? "" : reader.GetValue(33))
                    };
                    break;
                default: throw new ArgumentException("Wrong Account type", "type");
            }
            return account;
        }

        /// <summary>
        ///     обновляє Account
        ///     Хоча б один з boolівських параметрів має бути істинним
        /// </summary>
        /// <param name="account">Об'єкт дані якого обновляться в базі</param>
        /// <param name="updateAccount">обновити базовий аккаунт</param>
        /// <param name="update">Обновити підтаблицю</param>
        public static void UpdateAccount(Account account, bool updateAccount, bool update)
        {
            if (!update & !updateAccount)
                throw new ArgumentException("Хоча б один параметр параметр повинен true");
            SqlCommand query = null;
            if (update)
            {
                switch (account.AccountType)
                {
                    case AccountType.Student:
                        Student student = (Student)account;
                        int groupId = GetGroupIdByName(student.Group);
                        if (groupId < 0)
                            throw new ArgumentOutOfRangeException("account", "group not found");
                        query = new SqlCommand("UPDATE Student SET RBNumber = @RBNumber, SpecialityCode = @SpecialityCode, GroupId = @GroupId, FacultyId = @FacultyId, EFormId = @EFormId, ELevelId = @ELevelId, CountA = @CountA, CountB = @CountB, CountC = @CountC, CountD = @CountD, CountE = @CountE  WHERE StudentId = @StudentId");
                        query.Parameters.Add("@RBNumber", SqlDbType.VarChar, 10).Value = student.RBNumber;
                        query.Parameters.Add("@SpecialityCode", SqlDbType.VarChar, 10).Value = student.SpecialityCode;
                        query.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupId;
                        query.Parameters.Add("@FacultyId", SqlDbType.Int).Value = student.Faculty;
                        query.Parameters.Add("@EFormId", SqlDbType.Int).Value = student.EducationForm;
                        query.Parameters.Add("@ELevelId", SqlDbType.Int).Value = student.EducationLevel;
                        query.Parameters.Add("@CountA", SqlDbType.Int).Value = student.Acount;
                        query.Parameters.Add("@CountB", SqlDbType.Int).Value = student.Bcount;
                        query.Parameters.Add("@CountC", SqlDbType.Int).Value = student.Ccount;
                        query.Parameters.Add("@CountD", SqlDbType.Int).Value = student.Dcount;
                        query.Parameters.Add("@CountE", SqlDbType.Int).Value = student.Ecount;
                        query.Parameters.Add("@StudentId", SqlDbType.Int).Value = student.StudentId;
                        break;
                    case AccountType.Instructor:
                        Instructor instructor = (Instructor)account;
                        query = new SqlCommand("UPDATE Instructor SET Position = @Position, AcademicStatus = @AcademicStatus, AcademicLevel = @AcademicLevel, WorkPlace = @Workplace WHERE InstructorId = @InstructorId");
                        query.Parameters.Add("@Position", SqlDbType.VarChar, 100).Value = instructor.Position;
                        query.Parameters.Add("@AcademicStatus", SqlDbType.VarChar, 100).Value = instructor.AcademicStatus;
                        query.Parameters.Add("@AcademicLevel", SqlDbType.VarChar, 100).Value = instructor.AcademicLevel;
                        query.Parameters.Add("@WorkPlace", SqlDbType.VarChar, 100).Value = instructor.Workplace;
                        query.Parameters.Add("@InstructorId", SqlDbType.Int).Value = instructor.InstructorId;
                        break;
                    case AccountType.Normokontroler:
                        Normokontroler normokontroler = (Normokontroler)account;
                        query = new SqlCommand("UPDATE Normokontroler SET Position = @Position, WorkPlace = @WorkPlace WHERE NormokontrolerId = @NormokontrolerId");
                        query.Parameters.Add("@Position", SqlDbType.VarChar, 100).Value = normokontroler.Position;
                        query.Parameters.Add("@WorkPlace", SqlDbType.VarChar, 100).Value = normokontroler.Workplace;
                        query.Parameters.Add("@NormokontrolerId", SqlDbType.Int).Value = normokontroler.NormokontrolerId;
                        break;
                    default: throw new ArgumentException("Wrong Account type", "account");
                }
            }
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        if (updateAccount)
                        {
                            SqlCommand updateBase = new SqlCommand("UPDATE Account SET Login = @Login, Password = @Password, FirstName = @FirstName, LastName = @LastName, Patronymic = @Patronymic, Phone = @Phone, Address = @Address, Email = @Email WHERE AccountId = @AccountId", connection, transaction);
                            updateBase.Parameters.Add("@Login", SqlDbType.VarChar, 30).Value = account.Login;
                            updateBase.Parameters.Add("@Password", SqlDbType.VarChar, 30).Value = account.Password;
                            updateBase.Parameters.Add("@FirstName", SqlDbType.VarChar, 30).Value = account.Firstname;
                            updateBase.Parameters.Add("@LastName", SqlDbType.VarChar, 30).Value = account.Lastname;
                            updateBase.Parameters.Add("@Patronymic", SqlDbType.VarChar, 30).Value = account.Patronymic;
                            updateBase.Parameters.Add("@Phone", SqlDbType.VarChar, 20).Value = account.PhoneNumber;
                            updateBase.Parameters.Add("@Address", SqlDbType.VarChar, 100).Value = account.Address;
                            updateBase.Parameters.Add("@Email", SqlDbType.VarChar, 30).Value = account.Email;
                            updateBase.Parameters.Add("@AccountId", SqlDbType.Int).Value = account.ID;
                            updateBase.ExecuteNonQuery();
                        }
                        if (update)
                        {
                            query.Connection = connection;
                            query.Transaction = transaction;
                            query.ExecuteNonQuery();
                        }
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>Видаляє запис з таблиці облікових записів</summary>
        /// <param name="id">id аккаунту користувача</param>
        public static void DeleteAccount(int id)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {

                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (
                    SqlCommand query = new SqlCommand(string.Format("DELETE FROM Account WHERE AccountId = @Id"),
                        connection, transaction))
                {
                    try
                    {
                        SqlCommand subQuery = new SqlCommand("SELECT AccountType FROM Account WHERE AccountId in (@Id)", connection, transaction);
                        subQuery.Parameters.Add("Id", SqlDbType.Int).Value = id;
                        AccountType type = (AccountType)(subQuery.ExecuteScalar() ?? 0);
                        List<Project> projects = null;
                        if (type == AccountType.Student)
                            projects = GetProjects(id);
                        query.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                        query.ExecuteNonQuery();
                        if (type == AccountType.Student && projects != null)
                        {
                            subQuery = new SqlCommand("SELECT COUNT(*) FROM Student_Project WHERE ProjectId = @Id");
                            foreach (var project in projects)
                            {
                                if (project.Type != EProjectType.DiplomaProject)
                                    DeleteProject(project.ID);
                                else
                                {
                                    if (((int)subQuery.ExecuteScalar()) == 0)
                                        DeleteProject(project.ID);
                                }
                            }
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
        #endregion

        #region private methods related with groups
        private static string GetGroupById(int id)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand query = new SqlCommand("SELECT Name FROM [Group] WHERE GroupId = @Id", connection))
                {
                    query.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    return (string)(query.ExecuteScalar());
                }
            }
        }

        private static int GetGroupIdByName(string name)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand query = new SqlCommand("SELECT GroupId FROM [Group] WHERE Name = @name", connection))
                {
                    query.Parameters.Add("@name", SqlDbType.VarChar).Value = name;
                    return (int)(query.ExecuteScalar() ?? -1);
                }
            }
        }
        #endregion

        #region methods for events
        /// <summary>Додає event в базу</summary>
        /// <param name="event">Об'єкт дані якого заносяться в базу</param>
        /// <exception cref="ArgumentException">Якщо поле дедлайн знаходиться за межами типу SqlDateTime</exception>
        public static void AddEvent(Event @event)
        {
            if (@event.DeadLine < (DateTime)SqlDateTime.MinValue || @event.DeadLine > (DateTime)SqlDateTime.MaxValue)
                throw new ArgumentException(
                    string.Format(
                        "Невірний формат дати для поля дедлайн, значення повинно бути в межах: від {0} до {1}",
                        SqlDateTime.MinValue, SqlDateTime.MaxValue), "event");


            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand(
                    "INSERT INTO Event(SerialNumber, Title, Description, DeadLine, AcceptDate, Mark, Penalty, ProjectId, EventStatus) Values(@SerialNumber, @Title, @Description, @DeadLine, @AcceptDate, @Mark, @Penalty, @ProjectId, @EventStatus); SELECT CAST(scope_identity() AS int)",
                    connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@SerialNumber", SqlDbType.Int).Value = @event.SerialNumber;
                        query.Parameters.Add("@Title", SqlDbType.VarChar, 50).Value = @event.Title;
                        query.Parameters.Add("@Description", SqlDbType.Text).Value = @event.Description;
                        query.Parameters.Add("@DeadLine", SqlDbType.DateTime).Value = @event.DeadLine;
                        query.Parameters.Add("@AcceptDate", SqlDbType.DateTime).Value = @event.AcceptDate < (DateTime)SqlDateTime.MinValue ? DBNull.Value : (object)@event.AcceptDate;
                        query.Parameters.Add("@Mark", SqlDbType.Decimal).Value = @event.Mark;
                        query.Parameters.Add("@Penalty", SqlDbType.Real).Value = @event.Penalty;
                        query.Parameters.Add("@ProjectId", SqlDbType.Int).Value = @event.ProjectId;
                        query.Parameters.Add("@EventStatus", SqlDbType.Int).Value = @event.EStatus;
                        @event.ID = (int)query.ExecuteScalar();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>Обновляє подію по переданому об'єкту події</summary>
        /// <param name="event">Подія дані якої необхідно обновити в базі</param>
        /// <exception cref="ArgumentException">Якщо поле дедлайн знаходиться за межами типу SqlDateTime</exception>
        public static void UpdateEvent(Event @event)
        {
            if (@event.DeadLine < (DateTime)SqlDateTime.MinValue || @event.DeadLine > (DateTime)SqlDateTime.MaxValue)
                throw new ArgumentException(string.Format("Невірний формат дати для поля дедлайн, значення повинно бути в межах: від {0} до {1}", SqlDateTime.MinValue, SqlDateTime.MaxValue), "event");
            if (@event.ID < 1)
                throw new ArgumentException("невірний id події", "event");

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (
                    SqlCommand query =
                        new SqlCommand(
                            "UPDATE Event SET ProjectId = @ProjectId, SerialNumber = @SerialNumber, Title = @Title, Description = @Description, DeadLine = @DeadLine, AcceptDate = @AcceptDate, EventStatus = @EventStatus, Mark = @Mark, Penalty = @Penalty WHERE EventId = @Id",
                            connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@ProjectId", SqlDbType.Int).Value = @event.ProjectId;
                        query.Parameters.Add("@SerialNumber", SqlDbType.Int).Value = @event.SerialNumber;
                        query.Parameters.Add("@Title", SqlDbType.VarChar, 50).Value = @event.Title;
                        query.Parameters.Add("@Description", SqlDbType.Text).Value = @event.Description;
                        query.Parameters.Add("@DeadLine", SqlDbType.DateTime).Value = @event.DeadLine;
                        query.Parameters.Add("@AcceptDate", SqlDbType.DateTime).Value = @event.AcceptDate < (DateTime)SqlDateTime.MinValue ? DBNull.Value : (object)@event.AcceptDate;
                        query.Parameters.Add("@EventStatus", SqlDbType.Int).Value = @event.EStatus;
                        query.Parameters.Add("@Mark", SqlDbType.Int).Value = @event.EStatus;
                        query.Parameters.Add("@Penalty", SqlDbType.Real).Value = @event.Penalty;
                        query.Parameters.Add("@Id", SqlDbType.Int).Value = @event.ID;
                        query.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>Видаляє подію, з таблиці подій</summary>
        /// <param name="eventId">Id запису в таблиці</param>
        public static void DeleteEvent(int eventId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (
                    SqlCommand query = new SqlCommand("DELETE FROM Event WHERE EventId = @EventId", connection,
                        transaction))
                {
                    try
                    {
                        query.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;
                        query.ExecuteNonQuery();

                        transaction.Commit();
                        if (OnEventDelete != null)
                            OnEventDelete(eventId);
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        ///     Отримує всі події зв'язані з вказаним id проекту
        /// </summary>
        /// <param name="GroupId">id запису в таблиці проектів</param>
        /// <returns> List<Event> </returns>
        public static List<Event> GetEvents(int projectId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand query = new SqlCommand("SELECT * FROM Event WHERE ProjectId = @ProjectId", connection))
                {
                    query.Parameters.Add("ProjectId", SqlDbType.Int).Value = projectId;
                    using (SqlDataReader reader = query.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return null;

                        List<Event> events = new List<Event>();
                        while (reader.Read())
                            events.Add(new Event((int)reader["ProjectId"], (int)reader["SerialNumber"],
                                (string)reader["Title"],
                                (DateTime)reader["DeadLine"], (string)reader["Description"])
                            {
                                ID = (int)reader["EventId"],
                                AcceptDate = (DateTime)(!reader.IsDBNull(reader.GetOrdinal("AcceptDate")) ? reader["AcceptDate"] : new DateTime()),
                                EStatus = (EventStatus)reader["EventStatus"],
                                Mark = (float)reader["Mark"],
                                Penalty = (float)reader["Penalty"],
                                InputFiles = GetEventFileDictionary((int)reader["EventId"], EventFileType.Input),
                                OutputFiles = GetEventFileDictionary((int)reader["EventId"], EventFileType.Output)
                            });
                        return events;
                    }
                }
            }
        }

        private static Dictionary<string, string> GetEventFileDictionary(int eventId, EventFileType type)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (
                    SqlCommand query =
                        new SqlCommand(
                            "SELECT FilePath, FileKey FROM EventFile WHERE FileType = @Type AND EventId = @EventId",
                            connection))
                {
                    query.Parameters.Add("@Type", SqlDbType.Int).Value = type;
                    query.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;
                    using (SqlDataReader reader = query.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return null;

                        Dictionary<string, string> files = new Dictionary<string, string>();
                        while (reader.Read())
                            files.Add((string)reader["FileKey"], (string)reader["FilePath"]);

                        return files;
                    }
                }
            }
        }

        public static void UploadFile(int eventId, string filepath, string fileKey, EventFileType type)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand("INSERT INTO EventFile(EventId, FileType, FileKey, FilePath) Values(@EventId, @FileType, @FileKey, @FilePath)", connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@EventId", SqlDbType.Int).Value = eventId;
                        query.Parameters.Add("@FileType", SqlDbType.Int).Value = (int)type;
                        query.Parameters.Add("@FileKey", SqlDbType.VarChar, 100).Value = fileKey;
                        query.Parameters.Add("@FilePath", SqlDbType.VarChar, 100).Value = filepath;
                        query.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }

                }
            }
        }
        #endregion

        /// <summary>
        ///     Додає студента до проекту
        ///     //якщо передані id не 
        /// </summary>
        /// <param name="subjectId">subjectId з таблиці student</param>
        /// <param name="GroupId">GroupId з таблиці project</param>
        public static void AddStudentToProject(int studentId, int projectId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand count = new SqlCommand("SELECT COUNT(*) FROM Student_Project WHERE StudentId = @StudentId AND ProjectId = @ProjectId", connection);
                count.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                count.Parameters.Add("@ProjectId", SqlDbType.Int).Value = projectId;
                if (((int)count.ExecuteScalar()) > 0)
                    return;
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand("INSERT INTO Student_Project(StudentId, ProjectId) Values(@StudentId, @ProjectId)", connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                        query.Parameters.Add("@ProjectId", SqlDbType.Int).Value = projectId;
                        query.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Відв'язує студента від проекту
        /// </summary>
        /// <param name="studentId"></param>
        /// <param name="projectId"></param>
        public static void DeleteStudentFromProject(int studentId, int projectId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand("DELETE FROM Student_Project WHERE StudentId = @StudentId AND ProjectId = @ProjectId", connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                        query.Parameters.Add("@ProjectId", SqlDbType.Int).Value = projectId;
                        query.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void AddProject(Project project)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand("INSERT INTO Project(Theme, InstructorId, ProjectType, ProjectStatus, Description) VALUES(@Theme, @InstructorId, @ProjectType, @ProjectStatus, @Description); SELECT CAST(scope_identity() AS int)", connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@Theme", SqlDbType.VarChar, 100).Value = project.Theme;
                        query.Parameters.Add("@ProjectStatus", SqlDbType.Int).Value = (int)project.PStatus;
                        query.Parameters.Add("@InstructorId", SqlDbType.Int).Value = project.InstructorId;
                        query.Parameters.Add("@Description", SqlDbType.Text).Value = project.Description;
                        query.Parameters.Add("@ProjectType", SqlDbType.Int).Value = (int)project.Type;
                        project.ID = (int)query.ExecuteScalar();

                        if (project.Type == EProjectType.DiplomaProject)
                        {
                            DiplomaProject diploma = (DiplomaProject)project;
                            int instructorId = GetInstructorIdByName(diploma.InstroctorName);
                            if (instructorId < 0)
                                throw new ArgumentException("не знайдено id викладача", "project");
                            int normokontrolerId = GetNormokontrolerIdByName(diploma.NormokontrolerName);
                            if (normokontrolerId < 0)
                                throw new ArgumentException("не знайдено id нормоконтролера", "project");

                            SqlCommand diplomaInsert = new SqlCommand("INSERT INTO DiplomaProject(ProjectId, NormokontrolerId, Classification, NumberOfPages, NumberOfPictures, NumberOfTables, NumberOfFormuls, NumberOfLiterature, NumberOfPosters) VALUES(@ProjectId, @NormokontrolerId, @Classification, @NumberOfPages, @NumberOfPictures, @NumberOfTables, @NumberOfFormulas, @NumberOfLiterature, @NumberOfPosters); SELECT CAST(scope_identity() AS int)", connection, transaction);
                            diplomaInsert.Parameters.Add("@ProjectId", SqlDbType.Int).Value = project.ID;
                            diplomaInsert.Parameters.Add("@Classification", SqlDbType.VarChar, 50).Value = (diploma.Classification ?? "");
                            diplomaInsert.Parameters.Add("@NumberOfPages", SqlDbType.Int).Value = diploma.NumberOfPages;
                            diplomaInsert.Parameters.Add("@NumberOfPictures", SqlDbType.Int).Value = diploma.NumberOfPictures;
                            diplomaInsert.Parameters.Add("@NumberOfTables", SqlDbType.Int).Value = diploma.NumberOfTables;
                            diplomaInsert.Parameters.Add("@NumberOfFormulas", SqlDbType.Int).Value = diploma.NumberOfFormuls;
                            diplomaInsert.Parameters.Add("@NumberOfLiterature", SqlDbType.Int).Value = diploma.NumberOfLiterature;
                            diplomaInsert.Parameters.Add("@NumberOfPosters", SqlDbType.Int).Value = diploma.NumberOfPosters;
                            diplomaInsert.Parameters.Add("@NormokontrolerId", SqlDbType.Int).Value = normokontrolerId;

                            ((DiplomaProject)project).DiplomaId = (int)diplomaInsert.ExecuteScalar();
                        }
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }

                    transaction.Commit();
                }
            }
        }

        public static void UpdateProject(Project project)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    try
                    {
                        SqlCommand query = new SqlCommand("UPDATE Project SET Theme = @Theme, ProjectStatus = @ProjectStatus, Description = @Description  WHERE ProjectId = @Id", connection, transaction);
                        query.Parameters.Add("@Theme", SqlDbType.VarChar, 100).Value = project.Theme;
                        query.Parameters.Add("@Description", SqlDbType.Text).Value = project.Description;
                        query.Parameters.Add("@ProjectStatus", SqlDbType.Int).Value = (int)project.PStatus;
                        query.Parameters.Add("@Id", SqlDbType.Int).Value = project.ID;
                        query.ExecuteNonQuery();

                        switch (project.Type)
                        {
                            case EProjectType.DiplomaProject:
                                DiplomaProject diploma = (DiplomaProject)project;
                                int instructorId = GetInstructorIdByName(diploma.InstroctorName);
                                if (instructorId < 0)
                                    throw new ArgumentException("не знайдено id викладача", "project");
                                int normokontrolerId = GetNormokontrolerIdByName(diploma.NormokontrolerName);
                                if (normokontrolerId < 0)
                                    throw new ArgumentException("не знайдено id нормоконтролера", "project");
                                query = new SqlCommand("UPDATE DiplomaProject SET Classification = @Classification, NumberOfPages = @NumberOfPages, NumberOfPictures = @NumberOfPictures, NumberOfTables = @NumberOfTables, NumberOfFormuls = @NumberOfFormuls, NumberOfLiterature = @NumberOfLiterature, NumberOfPosters = @NumberOfPosters, InstructorId = @InstructorId, NormokontrolerId = @NormokontrolerId WHERE DiplomaId = @id", connection, transaction);

                                query.Parameters.Add("@Classification", SqlDbType.VarChar, 50).Value = diploma.Classification;
                                query.Parameters.Add("@NumberOfPages", SqlDbType.Int).Value = diploma.NumberOfPages;
                                query.Parameters.Add("@NumberOfPictures", SqlDbType.Int).Value = diploma.NumberOfPictures;
                                query.Parameters.Add("@NumberOfTables", SqlDbType.Int).Value = diploma.NumberOfTables;
                                query.Parameters.Add("@NumberOfFormuls", SqlDbType.Int).Value = diploma.NumberOfFormuls;
                                query.Parameters.Add("@NumberOfLiterature", SqlDbType.Int).Value = diploma.NumberOfLiterature;
                                query.Parameters.Add("@NumberOfPosters", SqlDbType.Int).Value = diploma.NumberOfPosters;
                                query.Parameters.Add("@InstructorId", SqlDbType.Int).Value = instructorId;
                                query.Parameters.Add("@NormokontrolerId", SqlDbType.Int).Value = normokontrolerId;
                                query.Parameters.Add("@id", SqlDbType.Int).Value = diploma.ID;
                                query.ExecuteNonQuery();
                                break;
                            case EProjectType.LabWork: break;
                            case EProjectType.Rgr: break;
                            default: throw new ArgumentException("Wrong project type", "project");
                        }
                        query.Dispose();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }

                }
            }
        }

        public static void DeleteProject(int projectId)
        {
            var events = GetEvents(projectId);
            if (events != null)
                foreach (var @event in GetEvents(projectId))
                    DeleteEvent(@event.ID);
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand("DELETE FROM Project WHERE ProjectId = @id", connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@id", SqlDbType.Int).Value = projectId;
                        query.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static List<Project> GetProjects(int studentId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand query = new SqlCommand("SELECT * FROM Student_Project JOIN Project on Project.ProjectId = Student_Project.ProjectId LEFT JOIN DiplomaProject on (Project.ProjectId = DiplomaProject.ProjectId AND ProjectType = 3) WHERE StudentId = @StudentId", connection))
                {
                    query.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                    using (SqlDataReader reader = query.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return null;

                        List<Project> projects = new List<Project>();
                        while (reader.Read())
                        {
                            EProjectType projectType = (EProjectType)reader["ProjectType"];
                            switch (projectType)
                            {
                                case EProjectType.LabWork:
                                    projects.Add(new LabWorks((int)reader["InstructorId"], (string)reader["Theme"], (string)reader["Desription"])
                                    {
                                        ID = (int)reader["ProjectId"],
                                        InstructorId = (int)reader["InstructorId"],
                                        PStatus = (ProjectStatus)reader["ProjectStatus"],
                                        Events = GetEvents((int)reader["ProjectId"]),

                                    });
                                    break;
                                case EProjectType.Rgr: projects.Add(new RGR((int)reader["InstructorId"], (string)reader["Theme"], (string)reader["Description"])
                                {
                                    ID = (int)reader["ProjectId"],
                                    PStatus = (ProjectStatus)reader["ProjectStatus"],
                                    Events = GetEvents((int)reader["ProjectId"])
                                });
                                    break;
                                case EProjectType.DiplomaProject: projects.Add(new DiplomaProject((int)reader["InstructorId"], (string)reader["Theme"], (string)reader["Description"])
                                {
                                    ID = (int)reader["ProjectId"],
                                    DiplomaId = (int)reader["DiplomaId"],
                                    PStatus = (ProjectStatus)reader["ProjectStatus"],
                                    Events = GetEvents((int)reader["ProjectId"]),
                                    Classification = (string)reader["Classification"],
                                    NumberOfPages = (int)reader["NumberOfPages"],
                                    NumberOfPictures = (int)reader["NumberOfPictures"],
                                    NumberOfTables = (int)reader["NumberOfTables"],
                                    NumberOfFormuls = (int)reader["NumberOfFormuls"],
                                    NumberOfLiterature = (int)reader["NumberOfLiterature"],
                                    NumberOfPosters = (int)reader["NumberOfPosters"],
                                    InstroctorName = GetInstructorName((int)reader["InstructorId"]),
                                    NormokontrolerName = GetNormokontrolerName((int)reader["NormokontrolerId"])
                                });
                                    break;
                                default: throw new ArgumentException("wrong project type");
                            }
                        }
                        return projects;
                    }
                }
            }
        }

        private static string GetInstructorName(int instructorId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand query = new SqlCommand("SELECT (LastName + ' ' + FirstName + ' ' + Patronymic) as Name FROM Account JOIN Instructor on Account.AccountId = Instructor.AccountId WHERE Instructor.InstructorId = @id", connection))
                {
                    query.Parameters.Add("@id", SqlDbType.Int).Value = instructorId;
                    return (string)query.ExecuteScalar();
                }
            }
        }

        private static string GetNormokontrolerName(int normokontrolerId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand query = new SqlCommand("SELECT (LastName + ' ' + FirstName + ' ' + Patronymic) as Name FROM Account JOIN Normokontroler on Account.AccountId = Normokontroler.AccountId WHERE Normokontroler.NormokontrolerId = @id", connection))
                {
                    query.Parameters.Add("@id", SqlDbType.Int).Value = normokontrolerId;
                    return (string)query.ExecuteScalar();
                }
            }
        }

        /// <summary>
        ///     Отримую InstructorId по ФІБ(Конкатенація через пробіл)
        /// </summary>
        /// <param name="name">ФІБ</param>
        /// <returns>InstructorId чи -1 якщо не знайдено</returns>
        private static int GetInstructorIdByName(string name)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand query = new SqlCommand("SELECT InstructorId FROM Instructor JOIN Account on Account.AccountId = Instructor.AccountId WHERE (LastName + ' ' + FirstName + ' ' + Patronymic = @Name)", connection))
                {
                    query.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
                    return (int)(query.ExecuteScalar() ?? -1);
                }
            }
        }

        /// <summary>
        ///     Отримую NormokontrolerId по ФІБ(Конкатенація через пробіл)
        /// </summary>
        /// <param name="name">ФІБ</param>
        /// <returns>NormokontrolerId чи -1 якщо не знайдено</returns>
        private static int GetNormokontrolerIdByName(string name)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand query = new SqlCommand("SELECT NormokontrolerId FROM Normokontroler JOIN Account on Account.AccountId = Normokontroler.AccountId WHERE (LastName + ' ' + FirstName + ' ' + Patronymic = @Name)", connection))
                {
                    query.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
                    return (int)(query.ExecuteScalar() ?? -1);
                }
            }
        }

        /// <summary>
        /// Отримуємо список студентів по заданому id групи
        /// </summary>
        /// <param name="group">id запису групи в таблиці груп</param>
        /// <returns>List<Student></returns>
        public static List<Student> GetStudentsByGroup(string group)
        {
            int groupId = GetGroupIdByName(group);
            if (groupId == -1)
                return null;

            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand query = new SqlCommand("SELECT * FROM Student JOIN Account on Account.AccountId = Student.AccountId WHERE Student.GroupId = @GroupId", connection))
                {
                    query.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupId;
                    using (SqlDataReader reader = query.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return null;
                        List<Student> students = new List<Student>();
                        while (reader.Read())
                        {
                            students.Add((Student)CreateAccountObject(reader, AccountType.Student));
                        }
                        return students;
                    }
                }
            }
        }

        public static List<Student> GetStudentsByProjectId(int projectId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand query = new SqlCommand("SELECT * FROM Student_Project JOIN Student on Student_Project.StudentId = Student.StudentId JOIN Account on Account.AccountId = Student.AccountId WHERE Student_Project.ProjectId = @ProjectId", connection))
                {
                    query.Parameters.Add("@ProjectId", SqlDbType.Int).Value = projectId;
                    using (SqlDataReader reader = query.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return null;
                        List<Student> students = new List<Student>();
                        while (reader.Read())
                            students.Add((Student)CreateAccountObject(reader, AccountType.Student));

                        return students;
                    }
                }
            }
        }

        public static List<Student> GetStudentsByInstructorId(int instructorId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand query = new SqlCommand(@"SELECT Account.AccountId
FROM Account
JOIN Student ON(Student.AccountId = Account.AccountId)
JOIN Student_Project ON(Student.StudentId = Student_Project.StudentId)
JOIN Project ON (Project.ProjectId = Student_Project.ProjectId)
JOIN Instructor ON (Project.InstructorId = Instructor.InstructorId)
WHERE Instructor.InstructorId = @Id
UNION
SELECT Account.AccountId
FROM Account
JOIN Student ON(Student.AccountId = Account.AccountId)
JOIN [Group] ON(Student.GroupId = [Group].GroupId)
JOIN Subject_Group ON([Group].GroupId = Subject_Group.GroupId)
JOIN [Subject] ON ([Subject].SubjectId = Subject_Group.SubjectId)
JOIN Instructor ON (Instructor.InstructorId = [Subject].InstructorId)
WHERE Instructor.InstructorId = @Id", connection))
                {
                    query.Parameters.Add("@Id", SqlDbType.Int).Value = instructorId;
                    using (SqlDataReader reader = query.ExecuteReader())
                    {
                        if (!reader.HasRows)
                            return null;
                        List<Student> students = new List<Student>();
                        while (reader.Read())
                            students.Add((Student)GetAccountbyId((int)reader["AccountId"]));
                        return students;
                    }
                }
            }
        }

        /// <summary>
        ///     Додає нову групу у список груп
        /// </summary>
        /// <param name="name">Назва групи</param>
        public static void AddGroup(string name)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand("INSERT INTO [Group](Name) VALUES(@Name)", connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@Name", SqlDbType.VarChar, 10).Value = name;
                        query.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }

        }

        /// <summary>
        ///  Обновляє групу в заданого студента
        /// </summary>
        /// <param name="subjectId">id з таблиці студентів</param>
        /// <param name="groupId">id з таблиці груп</param>
        public static void UpdateStudentGroup(int studentId, int groupId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand("UPDATE Student SET GroupId = @GroupId WHERE StudentId = @StudentId", connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@StudentId", SqlDbType.Int).Value = studentId;
                        query.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupId;
                        query.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        ///     Видаляємо групу
        ///     Якщо є студенти цієї групи видалити не вдастся оскільки видалення заблокує Foreign Key
        /// </summary>
        /// <param name="id">Id запису групи в таблиці</param>
        public static void DeleteGroup(int id)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand("DELETE FROM [Group] WHERE GroupId = @Id", connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                        query.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Список груп для даного викладача
        /// </summary>
        /// <param name="instructorId">id викладача</param>
        /// <returns>Dictionary<string, int></returns>
        public static Dictionary<string, int> GetGroups(int instructorId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand query = new SqlCommand(@"SELECT [Group].GroupId,  [Group].Name 
FROM [Group]
JOIN Subject_Group on (Subject_Group.GroupId = [Group].GroupId) 
JOIN [Subject] on(Subject_Group.SubjectId = [Subject].SubjectId)
JOIN [Instructor] on(Instructor.InstructorId = [Subject].InstructorId)
WHERE Instructor.InstructorId = 1", connection))
            {
                connection.Open();
                using (SqlDataReader reader = query.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    Dictionary<string, int> groups = new Dictionary<string, int>();
                    while (reader.Read())
                        groups.Add((string)reader["Name"], (int)reader["GroupId"]);

                    return groups;
                }
            }
        }

        ///<summary>
        ///Повертає список всіх існуючих груп
        ///</summary>
        ///<returns>Dictionary<string, int></returns>
        public static Dictionary<string, int> GetGroups()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            using (SqlCommand query = new SqlCommand("SELECT GroupId, Name FROM [Group]", connection))
            {
                connection.Open();
                using (SqlDataReader reader = query.ExecuteReader())
                {
                    if (!reader.HasRows)
                        return null;
                    Dictionary<string, int> groups = new Dictionary<string, int>();
                    while (reader.Read())
                        groups.Add((string)reader["Name"], (int)reader["GroupId"]);

                    return groups;
                }
            }
        }

        public static void AddSubject(int instrutorId, string name)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand("INSERT INTO [Subject](InstructorId, Name) VALUES(@InstrutorId, @Name)", connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@Name", SqlDbType.VarChar, 10).Value = name;
                        query.Parameters.Add("@InstrutorId", SqlDbType.Int).Value = instrutorId;
                        query.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void DeleteSubject(int subjectId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand("DELETE FROM [Subject] WHERE SubjectId = @SubjectId", connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@SubjectId", SqlDbType.Int).Value = subjectId;
                        query.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void UpdateSubject(int subjectId, int instructorId, string name)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand("UPDATE Subject SET InstructorId = @InstructorId, Name = @Name WHERE SubjectId = @SubjectId", connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@SubjectId", SqlDbType.Int).Value = subjectId;
                        query.Parameters.Add("@InstructorId", SqlDbType.Int).Value = instructorId;
                        query.Parameters.Add("@Name", SqlDbType.VarChar).Value = name;
                        query.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void AddSubjectToGroup(int subjectId, int groupId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                SqlCommand count = new SqlCommand("SELECT COUNT(*) FROM Subject_Group WHERE SubjectId = @SubjectId AND GroupId = @GroupId", connection);
                count.Parameters.Add("@SubjectId", SqlDbType.Int).Value = subjectId;
                count.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupId;
                if (((int)count.ExecuteScalar()) > 0)
                    return;
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand("INSERT INTO Subject_Group(SubjectId, GroupId) Values(@SubjectId, @GroupId)", connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@SubjectId", SqlDbType.Int).Value = subjectId;
                        query.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupId;
                        query.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        public static void DeleteSubjectFromGroup(int subjectId, int groupId)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                using (SqlCommand query = new SqlCommand("DELETE FROM Subject_Group WHERE SubjectId = @SubjectId AND GroupId = @GroupId", connection, transaction))
                {
                    try
                    {
                        query.Parameters.Add("@Subject", SqlDbType.Int).Value = subjectId;
                        query.Parameters.Add("@GroupId", SqlDbType.Int).Value = groupId;
                        query.ExecuteNonQuery();
                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}
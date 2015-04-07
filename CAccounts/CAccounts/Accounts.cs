using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Text;

namespace CAccounts
{
    [Serializable]
    public enum AccountType
    {
        Undefined = 0, Student = 1, Instructor = 2, Normokontroler = 3
    }
    [Serializable]
    public enum Faculties { UNDEFINED, FIOT, FEL, FPM, TEF, RTF, FMF, HTF, FSP, FMM, FL, FEA, FBT, FAKS, IFF, PBF, ZF, IPSA };
    [Serializable]
    public enum EducationForms { UNDEFINED, DAILY, ABSENCE };
    [Serializable]
    public enum EducationLevels { UNDEFINED, BACHELOR, SPECIALIST, MASTER };
    [Serializable]
    public abstract class Account
    {
        private string _login;
        private string _password;
        private string _firstname;
        private string _lastname;
        private string _patronymic;
        private string _address;
        private string _phoneNumber;
        private string _email;
        private readonly AccountType _accountType;

        public AccountType AccountType { get { return _accountType; } }

        protected Account(string login, string password, string firstname, string lastname, string patronymic, AccountType accountType = AccountType.Undefined,
            string address = "", string phonenumber = "", string email = "")
        {
            _accountType = accountType;
            Login = login;
            Password = password;
            Firstname = firstname;
            Lastname = lastname;
            Patronymic = patronymic;
            Address = address;
            PhoneNumber = phonenumber;
            Email = email;
        }

        public static bool CheckString(string pattern, string value)
        {
            return new Regex(pattern).IsMatch(value);
        }
        public static string Format(string value)
        {
            string[] words = value.Split(' ');
            for (int i = 0; i < words.Length; i++)
                words[i] = (String.IsNullOrEmpty(words[i])) ? "" : words[i].First().ToString().ToUpper() + words[i].Substring(1, words[i].Length - 1).ToLower();
            StringBuilder sb = new StringBuilder();
            foreach (string word in words)
                sb.Append(String.IsNullOrEmpty(word) ? "" : (word + ' '));
            return sb.ToString();
        }
        public string Firstname
        {
            get { return _firstname; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("Поле імені має бути заповненим.");
                if (!CheckString(@"^[а-яА-ЯёЁa-zA-ZїЇіІ\W]+$", value))
                    throw new ArgumentException("Ім'я має складатися тільки з літер.");

                _firstname = Format(value);
            }
        }
        public string Lastname
        {
            get { return _lastname; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("Поле прізвища має бути заповненим.");
                if (!CheckString(@"^[а-яА-ЯёЁa-zA-ZїЇіІ\W]+$", value))
                    throw new ArgumentException("Прізвище має складатися тільки з літер.");
                _lastname = Format(value);
            }
        }
        public string Patronymic
        {
            get { return _patronymic; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("Поле імені по батькові має бути заповненим.");
                if (!CheckString(@"^[а-яА-ЯёЁa-zA-ZїЇіІ\W]+$", value))
                    throw new ArgumentException("Ім'я по батькові має складатися тільки з літер.");
                _patronymic = Format(value);
            }
        }
        public string Login
        {
            get { return _login; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("Логін має бути заповненим.");
                if (!CheckString(@"^[a-zA-Z][a-zA-Z0-9-_./\|<>@#$&*а-яА-ЯіІїЇёЁ]{1,20}$", value))
                    throw new ArgumentException("Логін містить недопустимі символи або має невірну довжину.");
                _login = value;
            }
        }
        public string Password
        {
            get { return _password; }
            set
            {
                if (value.Length < 6 || value.Length > 20)
                    throw new ArgumentException("Невірна довжина пароля! Пароль має бути довжиною від 6 до 20 символів.");
                _password = value;
            }
        }
        public string Fullname
        {
            get { return String.Format("{0} {1}. {2}.", Lastname, Firstname.First(), Patronymic.First()); }
        }
        public string Address
        {
            get { return _address; }
            set
            {
                // TODO
                _address = value;
            }
        }
        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    _phoneNumber = "";
                else
                {
                    string pattern = @"^[0-9]+$";
                    string newValue = value.Replace(" ", "");
                    if (!(new Regex(pattern).IsMatch(newValue)))
                        throw new ArgumentException("Номер телефона повинен містити тільки цифри.");
                    _phoneNumber = newValue;
                }
            }
        }
        public string Email
        {
            get { return _email; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    _email = "";
                else
                {
                    string newValue = value.Replace(" ", "");
                    if (!(new EmailAddressAttribute().IsValid(newValue)))
                        throw new ArgumentException("Невірне значення e-mail.");
                    _email = newValue;
                }
            }
        }
        public int ID { get; set; }
    }

    [Serializable]
    public class Student : Account
    {
        public int StudentId { get; set; }
        private string _rbNumber;
        private string _specialityCode;
        private string _group;

        public Student(string login, string password, string firstname, string lastname, string patronymic, string rbNumber,
            string group, Faculties faculty = Faculties.UNDEFINED, string address = "", string phonenumber = "", string email = "", string specialityCode = "",
            EducationForms educForm = EducationForms.UNDEFINED,
            EducationLevels educLevel = EducationLevels.UNDEFINED)
            : base(login, password, firstname, lastname, patronymic, AccountType.Student, address, phonenumber, email)
        {
            RBNumber = rbNumber;
            SpecialityCode = specialityCode;
            Group = group;
            Faculty = faculty;
            EducationForm = educForm;
            EducationLevel = educLevel;
        }

        public string RBNumber
        {
            get { return _rbNumber; }
            set
            {
                if (String.IsNullOrEmpty(value))
                    throw new ArgumentException("Ім'я залікової книжки має бути заповненим.");
                _rbNumber = value;
            }
        }
        public string SpecialityCode
        {
            get { return _specialityCode; }
            set
            {
                // TODO
                _specialityCode = value;
            }
        }
        public string Group
        {
            get { return _group; }
            set
            {
                // TODO
                _group = value;
            }
        }
        public Faculties Faculty { get; set; }
        public EducationForms EducationForm { get; set; }
        public EducationLevels EducationLevel { get; set; }
        public int Acount { get; set; }
        public int Bcount { get; set; }
        public int Ccount { get; set; }
        public int Dcount { get; set; }
        public int Ecount { get; set; }
        public double AverageMark
        {
            get
            {
                return (double)(Acount * 5 + Bcount * 4 + Ccount * 4 + Dcount * 3 + Ecount * 3) /
                (Acount + Bcount + Ccount + Dcount + Ecount);
            }
        }
    }

    [Serializable]
    public class Instructor : Account
    {
        public int InstructorId { get; set; }
        public string Position { get; set; }
        public string AcademicStatus { get; set; }
        public string AcademicLevel { get; set; }
        public string Workplace { get; set; }

        public Instructor(string login, string password, string firstname, string lastname, string patronymic,
            string address = "", string phonenumber = "", string email = "", string position = "", string AcademStatus = "",
            string academLevel = "", string workplace = "")
            : base(login, password, firstname, lastname, patronymic, AccountType.Instructor, address, phonenumber, email)
        {
            Position = position;
            AcademicStatus = AcademStatus;
            AcademicLevel = academLevel;
            Workplace = workplace;
        }
    }

    [Serializable]
    public class Normokontroler : Account
    {
        public int NormokontrolerId { get; set; }
        public string Position { get; set; }
        public string Workplace { get; set; }

        public Normokontroler(string login, string password, string firstname, string lastname, string patronymic,
            string address = "", string phonenumber = "", string email = "", string position = "", string workplace = "")
            : base(login, password, firstname, lastname, patronymic, AccountType.Normokontroler, address, phonenumber, email)
        {
            Position = position;
            Workplace = workplace;
        }
    }
}
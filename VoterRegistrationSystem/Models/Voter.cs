
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Windows.Media;

namespace VoterRegistrationSystem.Models
{
    [Table("registrations")]
    public class Voter
    {
        [Key]
        public int VoterID { get; set; }
        public string VoterGuid { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string OtherNames { get; set; }
        public string NIN { get; set; }
        public string PassportID { get; set; }
        public string Occupation { get; set; }
        public int Age { get; set; }
        public int LgaID { get; set; }
        [ForeignKey("LgaID")]
        public virtual Lga Lga { get; set; }
        public int StateID { get; set; }
        [ForeignKey("StateID")]
        public virtual State State { get; set; }
        public int WardID { get; set; }
        [ForeignKey("WardID")]
        public virtual Ward Ward { get; set; }
        public int PollingUnitID { get; set; }
        [ForeignKey("PollingUnitID")]
        public virtual PollingUnit PollingUnit { get; set; }
        public int SexID { get; set; }
        [ForeignKey("SexID")]
        public virtual Sex Sex { get; set; }
        public int MaritalStatusID { get; set; }
        [ForeignKey("MaritalStatusID")]
        public virtual MaritalStatus MaritalStatus { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string HomeAddress { get; set; }
        public byte[] PhotoFront { get; set; }
        public virtual Fingerprint VoterFingerprint { get; set; }
        public string AddedBy { get; set; }
        public DateTime DateAdded { get; set; }
    }

    public class VoterViewModel : INotifyPropertyChanged
    {
        private int voterID;

        public int VoterID
        {
            get { return voterID; }
            set
            {
                if (voterID != value)
                {
                    voterID = value;
                    NotifyPropertyChanged("VoterID");
                }
            }
        }

        private string voterGuid;

        public string VoterGuid
        {
            get { return voterGuid; }
            set
            {
                if (voterGuid != value)
                {
                    voterGuid = value;
                    NotifyPropertyChanged("VoterGuid");
                }
            }
        }

        //private string firstName;

        //public string FirstName
        //{
        //    get { return firstName; }
        //    set
        //    {
        //        if (firstName != value)
        //        {
        //            firstName = value;
        //            NotifyPropertyChanged("FirstName");
        //        }
        //    }
        //}

        //private string surname;

        //public string Surname
        //{
        //    get { return surname; }
        //    set
        //    {
        //        if (surname != value)
        //        {
        //            surname = value;
        //            NotifyPropertyChanged("Surname");
        //        }
        //    }
        //}

        //private string otherNames;

        //public string OtherNames
        //{
        //    get { return otherNames; }
        //    set
        //    {
        //        if (otherNames != value)
        //        {
        //            otherNames = value;
        //            NotifyPropertyChanged("OtherNames");
        //        }
        //    }
        //}

        //private string passportID;

        //public string PassportID
        //{
        //    get { return passportID; }
        //    set
        //    {
        //        if (passportID != value)
        //        {
        //            passportID = value;
        //            NotifyPropertyChanged("PassportID");
        //        }
        //    }
        //}

        //private Lga lga;

        //public Lga Lga
        //{
        //    get { return lga; }
        //    set
        //    {
        //        if (lga != value)
        //        {
        //            lga = value;
        //            NotifyPropertyChanged("Lga");
        //        }
        //    }
        //}

        //private State state;

        //public State State
        //{
        //    get { return state; }
        //    set
        //    {
        //        if (state != value)
        //        {
        //            state = value;
        //            NotifyPropertyChanged("State");
        //        }
        //    }
        //}

        //private PollingUnit pollingUnit;

        //public PollingUnit PollingUnit
        //{
        //    get { return pollingUnit; }
        //    set
        //    {
        //        if (pollingUnit != value)
        //        {
        //            pollingUnit = value;
        //            NotifyPropertyChanged("PollingUnit");
        //        }
        //    }
        //}

        //private Sex sex;

        //public Sex Sex
        //{
        //    get { return sex; }
        //    set
        //    {
        //        if (sex != value)
        //        {
        //            sex = value;
        //            NotifyPropertyChanged("Sex");
        //        }
        //    }
        //}
        //private MaritalStatus maritalStatus;

        //public MaritalStatus MaritalStatus
        //{
        //    get { return maritalStatus; }
        //    set
        //    {
        //        if (maritalStatus != value)
        //        {
        //            maritalStatus = value;
        //            NotifyPropertyChanged("MaritalStatus");
        //        }
        //    }
        //}

        //private string phoneNumber;

        //public string PhoneNumber
        //{
        //    get { return phoneNumber; }
        //    set
        //    {
        //        if (phoneNumber != value)
        //        {
        //            phoneNumber = value;
        //            NotifyPropertyChanged("PhoneNumber");
        //        }
        //    }
        //}

        //private DateTime dateOfBirth;

        //public DateTime DateOfBirth
        //{
        //    get { return dateOfBirth; }
        //    set
        //    {
        //        if (dateOfBirth != value)
        //        {
        //            dateOfBirth = value;
        //            NotifyPropertyChanged("DateOfBirth");
        //        }
        //    }
        //}


        //private string homeAddress;

        //public string HomeAddress
        //{
        //    get { return homeAddress; }
        //    set
        //    {
        //        if (homeAddress != value)
        //        {
        //            homeAddress = value;
        //            NotifyPropertyChanged("HomeAddress");
        //        }
        //    }
        //}

        //private byte[] photoFront;

        //public byte[] PhotoFront
        //{
        //    get { return photoFront; }
        //    set
        //    {
        //        if (photoFront != value)
        //        {
        //            photoFront = value;
        //            NotifyPropertyChanged("PhotoFront");
        //        }
        //    }
        //}

        private Voter selectedVoter;

        public Voter SelectedVoter
        {
            get { return selectedVoter; }
            set
            {
                if (selectedVoter != value)
                {
                    selectedVoter = value;
                    NotifyPropertyChanged("SelectedVoter");
                }
            }
        }
        private ImageSource imageSource;

        public ImageSource ImageSource
        {
            get { return imageSource; }
            set
            {
                if (imageSource != value)
                {
                    imageSource = value;
                    NotifyPropertyChanged("ImageSource");
                }
            }
        }

        private ImageSource rightOneImageSource;

        public ImageSource RightOneImageSource
        {
            get { return rightOneImageSource; }
            set
            {
                if (rightOneImageSource != value)
                {
                    rightOneImageSource = value;
                    NotifyPropertyChanged("RightOneImageSource");
                }
            }
        }

        private ImageSource rightTwoImageSource;

        public ImageSource RightTwoImageSource
        {
            get { return rightTwoImageSource; }
            set
            {
                if (rightTwoImageSource != value)
                {
                    rightTwoImageSource = value;
                    NotifyPropertyChanged("RightTwoImageSource");
                }
            }
        }
        private ImageSource rightThreeImageSource;

        public ImageSource RightThreeImageSource
        {
            get { return rightThreeImageSource; }
            set
            {
                if (rightThreeImageSource != value)
                {
                    rightThreeImageSource = value;
                    NotifyPropertyChanged("RightThreeImageSource");
                }
            }
        }
        private ImageSource rightFourImageSource;

        public ImageSource RightFourImageSource
        {
            get { return rightFourImageSource; }
            set
            {
                if (rightFourImageSource != value)
                {
                    rightFourImageSource = value;
                    NotifyPropertyChanged("RightFourImageSource");
                }
            }
        }
        private ImageSource rightFiveImageSource;

        public ImageSource RightFiveImageSource
        {
            get { return rightFiveImageSource; }
            set
            {
                if (rightFiveImageSource != value)
                {
                    rightFiveImageSource = value;
                    NotifyPropertyChanged("RightFiveImageSource");
                }
            }
        }
        private ImageSource leftOneImageSource;

        public ImageSource LeftOneImageSource
        {
            get { return leftOneImageSource; }
            set
            {
                if (leftOneImageSource != value)
                {
                    leftOneImageSource = value;
                    NotifyPropertyChanged("LeftOneImageSource");
                }
            }
        }
        private ImageSource leftTwoImageSource;

        public ImageSource LeftTwoImageSource
        {
            get { return leftTwoImageSource; }
            set
            {
                if (leftTwoImageSource != value)
                {
                    leftTwoImageSource = value;
                    NotifyPropertyChanged("LeftTwoImageSource");
                }
            }
        }
        private ImageSource leftThreeImageSource;

        public ImageSource LeftThreeImageSource
        {
            get { return leftThreeImageSource; }
            set
            {
                if (leftThreeImageSource != value)
                {
                    leftThreeImageSource = value;
                    NotifyPropertyChanged("LeftThreeImageSource");
                }
            }
        }
        private ImageSource leftFourImageSource;

        public ImageSource LeftFourImageSource
        {
            get { return leftFourImageSource; }
            set
            {
                if (leftFourImageSource != value)
                {
                    leftFourImageSource = value;
                    NotifyPropertyChanged("LeftFourImageSource");
                }
            }
        }
        private ImageSource leftFiveImageSource;

        public ImageSource LeftFiveImageSource
        {
            get { return leftFiveImageSource; }
            set
            {
                if (leftFiveImageSource != value)
                {
                    leftFiveImageSource = value;
                    NotifyPropertyChanged("LeftFiveImageSource");
                }
            }
        }

        private ObservableCollection<Voter> searchResults;

        public ObservableCollection<Voter> SearchResults
        {
            get { return searchResults; }
            set
            {
                if (searchResults != value)
                {
                    searchResults = value;
                    NotifyPropertyChanged("SearchResults");
                }
            }
        }
        private ObservableCollection<Sex> sexes;

        public ObservableCollection<Sex> Sexes
        {
            get { return sexes; }
            set
            {
                if (sexes != value)
                {
                    sexes = value;
                    NotifyPropertyChanged("Sexes");
                }
            }
        }
        private ObservableCollection<MaritalStatus> maritalStatuses;

        public ObservableCollection<MaritalStatus> MaritalStatuses
        {
            get { return maritalStatuses; }
            set
            {
                if (maritalStatuses != value)
                {
                    maritalStatuses = value;
                    NotifyPropertyChanged("MaritalStatuses");
                }
            }
        }
        private ObservableCollection<Lga> lgas;

        public ObservableCollection<Lga> Lgas
        {
            get { return lgas; }
            set
            {
                if (lgas != value)
                {
                    lgas = value;
                    NotifyPropertyChanged("Lgas");
                }
            }
        }

        private ObservableCollection<State> states;

        public ObservableCollection<State> States
        {
            get { return states; }
            set
            {
                if (states != value)
                {
                    states = value;
                    NotifyPropertyChanged("States");
                }
            }
        }


        private ObservableCollection<Ward> wards;


        public ObservableCollection<Ward> Wards
        {
            get { return wards; }
            set
            {
                if (wards != value)
                {
                    wards = value;
                    NotifyPropertyChanged("Wards");
                }
            }
        }

        private ObservableCollection<PollingUnit> pollingunits;


        public ObservableCollection<PollingUnit> PollingUnits
        {
            get { return pollingunits; }
            set
            {
                if (pollingunits != value)
                {
                    pollingunits = value;
                    NotifyPropertyChanged("PollingUnits");
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String info)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(info));
            }
        }
    }
}
using System.Windows.Input;
using Google.Authenticator;
using MicroMvvm;

namespace TwoFactor.ViewModel
{
    public class MainViewModel : ObservableObject
    {
        private string _accountSecretKey = "TEST_TEST_TEST_TEST";
        public string QrCodeImageUrl { get; }
        public string ManualEntrySetupCode { get; }
        public string ValidateCode { get; set; }

        public bool IsCorrectPin { get; set; }
        public string ValidationResult { get; set; }

        public ICommand ValidateCommand { get; }

        public MainViewModel()
        {
            var authenticator = new TwoFactorAuthenticator();
            var setupInfo = authenticator.GenerateSetupCode("MyApp", "user@example.com", _accountSecretKey, 300, 300);
            QrCodeImageUrl = setupInfo.QrCodeSetupImageUrl;
            ManualEntrySetupCode = "Manual Setup Code: " + setupInfo.ManualEntryKey;
            ValidateCommand = new RelayCommand(Validate);
        }

        private void Validate()
        {
            SetPropertyValue(() => IsCorrectPin, new TwoFactorAuthenticator().ValidateTwoFactorPIN(_accountSecretKey, ValidateCode));
            SetPropertyValue("ValidationResult", IsCorrectPin ? "True" : "False");
            SetPropertyValue(() => ValidationResult, IsCorrectPin ? "True" : "False");
        }
    }
}

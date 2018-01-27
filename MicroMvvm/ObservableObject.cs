using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace MicroMvvm
{
    [Serializable]
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        protected void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpresssion)
        {
            var propertyName = PropertySupport.ExtractPropertyName(propertyExpresssion);
            RaisePropertyChanged(propertyName);
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            VerifyPropertyName(propertyName);
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected void SetPropertyValue<T>(Expression<Func<T>> property , object propertyValue, bool raisePropertyChanged = true)
        {
            var propertyInfo = ((MemberExpression)property.Body).Member as PropertyInfo;
            SetPropertyValue(propertyInfo, propertyValue, raisePropertyChanged);
        }

        protected void SetPropertyValue(PropertyInfo propertyInfo, object propertyValue, bool raisePropertyChanged = true)
        {
            if (propertyInfo?.CanWrite != true)
                return;

            propertyInfo.SetMethod.Invoke(this, new[] { propertyValue });

            if (raisePropertyChanged)
                RaisePropertyChanged(propertyInfo.Name);

        }
        protected void SetPropertyValue(string propertyName, object propertyValue, bool raisePropertyChanged = true)
            => SetPropertyValue(GetType().GetProperty(propertyName), propertyValue, raisePropertyChanged);

        /// <summary>
        /// Warns the developer if this Object does not have a public property with
        /// the specified name. This method does not exist in a Release build.
        /// </summary>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // verify that the property name matches a real,  
            // public, instance property on this Object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
                Debug.Fail("Invalid property name: " + propertyName);
        }
    }
}

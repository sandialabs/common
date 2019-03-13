namespace COMMONConfig.Frontend.Views.UserContols
{
    /// <summary>
    ///     Needed a way to know when to read the password out of a PasswordBox because WPF
    ///     doesn't allow data binding to a PasswordBox. Added this interface so we can
    ///     have our user controls extract the password at the appropriate time.
    /// </summary>
    internal interface IHandleDataBinding
    {
        void DoDataBinding();
    }
}
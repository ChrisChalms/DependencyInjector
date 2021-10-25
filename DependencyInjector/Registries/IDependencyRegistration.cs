namespace CC.DependencyInjector.Registries
{
    internal interface IDependencyRegistration
    {
        internal Container.RegistrationScope RegistrationScope { get; }
        internal object GetObject();
    }
}

namespace MAUIRecipeApp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            var window = base.CreateWindow(activationState);

            const int newWidth = 500;
            const int newHeight = 800;

            window.Width = newWidth;
            window.Height = newHeight;

            window.MinimumWidth = 450;
            window.MinimumHeight = newHeight;
            


            return window;
        }
    }
}

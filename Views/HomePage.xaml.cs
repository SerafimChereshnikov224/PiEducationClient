namespace PiClientV1.Views;

public partial class HomePage : ContentPage
{
    public HomePage()
    {
        InitializeComponent();
    }

    private async void OnNotationClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//NotationPage");
    }

    private async void OnTheoryClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//TheoryPage");
    }

    private async void OnProblemsClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//ProblemsPage");
    }

    private async void OnEmulatorClicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("//EmulatorPage");
    }

    private async void OnAboutClicked(object sender, EventArgs e)
    {
        await DisplayAlert("О системе",
            "π-Исчисление: Обучающая система\n\n" +
            "Разработчики: Сердюков Кирилл, Корнишев Даниил\n" +
            "Организация: МИФИ Лаборатория ФОИТ\n" +
            "Email: doddadid@gmail.com\n\n" +
            "Система предназначена для изучения π-исчисления Роберта Милнера.",
            "OK");
    }
}
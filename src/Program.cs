namespace Game4 {

public class Program {
    public static void Main(string[] args) {
        using (GameImpl game = new GameImpl()) {
            game.Run();
        }
    }
}

}

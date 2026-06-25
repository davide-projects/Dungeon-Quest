using DungeonQuest.Models;
using DungeonQuest.UI;

var nomeEroe = Benvenuto.ChiediNome();
var eroe = new Hero(nomeEroe);

var menu = new MenuManager(eroe);
menu.Run();

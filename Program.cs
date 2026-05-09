using System.IO;

class Program
{
    static void Main()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.White;
        Bank bank = new Bank();
        Utils.StartNotes();

        while (true)
        {
            bank.PlayTurn();
        }
    }
}

class Utils
{
    public static string GenerateCode()
    {
        Random num = new Random();
        int code = num.Next(1000, 10000);
        return code.ToString();
    }

    public static void StartNotes()
    {
        Console.WriteLine($"{new string('=', 57)}\n  THE BANK\n{new string('=', 57)}\n");
        Console.WriteLine($"  INSTRUCTIONS:");
        Console.WriteLine("  - To end the game, type \"quit\" when asked for an\n    action");
        Console.WriteLine("  - Items are listed in each room, an item's number is\n    it's position in the list");
        Console.WriteLine("  - If a guard finds you in a restricted area with no\n    disguise, they will catch you");
        Console.WriteLine("  - Guard uniforms can be used to walk through a\n    restricted with a guard, but they only last two\n    turns after equipped");
        Console.WriteLine("  - A new disguise can be equipped when the number\n    of turns left on the current one is shown as 0");
        Console.WriteLine("  - Lockpicks can be used to unlock doors but are\n    discarded after two uses");
        Console.WriteLine("  - The vault has no key, to open it find the code and\n    input it after walking to the vault");
        Console.WriteLine("  - To end the game walk back to the street with\n    everything you've taken");
        Console.WriteLine("  - Any item left on the street will also be counted\n    towards your total take\n");
        Console.WriteLine($"{new string('-', 57)}\n  **To move on from messages like this, press any key**\n{new string('-', 57)}");
        Console.ReadKey();
    }

    public static void Return()
    {
        Console.SetCursorPosition(0, Console.CursorTop - 1);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop);
    }

    public static void ErrorMessage(string s)
    {
        Console.Write($"**{s}**");
        Console.ReadKey();
        Console.SetCursorPosition(0, Console.CursorTop);
        Console.Write(new string(' ', Console.WindowWidth));
        Console.SetCursorPosition(0, Console.CursorTop);
        return;
    }

    public static bool HasBag(Inventory inventory)
    {
        foreach(Item i in inventory.GetInventory())
        {
            if (i is Bag) { return true; }
        }
        return false;
    }

    public static Bag BagPos(Inventory inventory)
    {
        int x = 0;
        foreach(Item i in inventory.GetInventory())
        {
            if (i is Bag) { break; }
            x++;
        }
        return (Bag)inventory.GetInventory()[x];
    }

    public static char InventoryOption(string s, Inventory inventory)
    {
        char output;
        string input;
        bool hasbag = Utils.HasBag(inventory);

        if (hasbag == true)
        {
            Console.Write($"{s} [I]nventory or [B]ag: ");
            while (true)
            {
                input = Console.ReadLine()!;
                Return();
                if (input.ToLower() == "b")
                {
                    output = 'b';
                    return output;
                }
                else if (input.ToLower() == "i")
                {
                    output = 'i';
                    return output;
                }
                else
                {
                    Console.Write($"Invalid input, {s} [I]nventory or [B]ag: ");
                }
            }
        }
        else { output = 'i'; return output; }
    }

    public static int Select(string s1, string s2, int n)
    {
        int choice;
        string input;

        Console.Write($"Enter the {s1} number to {s2}: ");
        while (true)
        {
            input = Console.ReadLine()!;
            Return();
            if (int.TryParse(input, out choice) && choice > 0 && choice <= n)
            {
                choice -= 1;
                return choice;              
            }
            Console.Write($"Invalid input, Enter the {s1} number to {s2}: ");
        }
    }
}

//Bank, contents and entities:

class Layout
{
    public Room rStreet   { get; } = new Room("Street", false, new List<Item> { });
    public Room rMain     { get; } = new Room("Main Entrance", false, new List<Item> { });
    public Room rDownstairs { get; } = new Room("Stairwell (Downstairs)", true,  new List<Item> { });
    public Room rUpstairs { get; } = new Room("Stairwell (Upstairs)", true,  new List<Item> { });
    public Room rLift     { get; } = new Room("Elevator", true,  new List<Item> { new Disguise() });
    public Room rLanding  { get; } = new Room("Landing", true,  new List<Item> { });
    public Room rLobby    { get; } = new Room("Lobby", false, new List<Item> { });
    public Room rTeller  { get; } = new Room("Teller Station", true,  new List<Item> { new Cash(), new Gold(), new Disguise(), new Cash(), new Lockpick() });
    public Room rHall     { get; } = new Room("Hall", true,  new List<Item> { });
    public Room rOffice   { get; } = new Room("Office", true,  new List<Item> { new Necklace(), new Cash() });
    public Room rBoard    { get; } = new Room("Board Room", true,  new List<Item> { new Watch(), new Bonds() });
    public Room rBreak    { get; } = new Room("Break Room", true,  new List<Item> { new Necklace(), new Watch() });
    public Room rStorage  { get; } = new Room("Storage room", true,  new List<Item> { new Lockpick(), new Disguise() });
    public Room rElectrical { get; } = new Room("Electrical", true,  new List<Item> { new Copper(), new Copper(), new Copper(), new Lockpick() });
    public Room rServer   { get; } = new Room("Server Room", true,  new List<Item> { new HardDrive() });
    public Room rCounting { get; } = new Room("Counting Room", true,  new List<Item> { new Cash(), new Bonds(), new Cash(), new Gold() });
    public Room rManager  { get; } = new Room("Manager's Office", true,  new List<Item> { new Watch(), new Bonds() });
    public Room rSecurity { get; } = new Room("Security Room", true,  new List<Item> { new Disguise(), new Lockpick() });
    public Room rSafeDep  { get; } = new Room("Safe Deposit Room", true,  new List<Item> { new Cash(), new Gold(), new Cash(), new Cash() });
    public Room rVault    { get; } = new Room("Vault", true,  new List<Item> { new Gold(), new Bonds(), new Necklace(), new Cash(), new Gold(), new Painting(), new Bonds(), new Gold(), new Disguise() });

    public Layout(string vaultCode, Bag bag)
    {
        rStreet.GetItemList().Add(bag);

        rSecurity.GetItemList().Add(new VaultCode(vaultCode));

        rStreet.CreateDoor(rMain, false);
        rMain.CreateDoor(rLobby, false);

        rLobby.CreateDoor(rHall, false);
        rLobby.CreateDoor(rOffice, false);
        rLobby.CreateDoor(rTeller, false);

        rHall.CreateDoor(rOffice, false);
        rHall.CreateDoor(rBoard, false);
        rHall.CreateDoor(rBreak, false);
        rHall.CreateDoor(rDownstairs, false);

        rOffice.CreateDoor(rSafeDep, true);
        rOffice.CreateDoor(rLift, false);

        rBreak.CreateDoor(rSecurity, true);
        rBreak.CreateDoor(rStorage, false);

        rDownstairs.CreateDoor(rUpstairs, false);
        rUpstairs.CreateDoor(rLanding, true);
        rLift.CreateDoor(rLanding, false);

        rLanding.CreateDoor(rManager, true);
        rLanding.CreateDoor(rServer, false);
        rLanding.CreateDoor(rElectrical, false);
        rLanding.CreateDoor(rCounting, true);

        rSafeDep.CreateVaultDoor(rVault, true, vaultCode);
    }
}


class GameState
{
    private List<Guard> guards = new List<Guard>();

    public GameState(Layout layout)
    {
        guards.Add(new Guard(layout.rHall, new List<Room> { layout.rHall, layout.rHall, layout.rLobby, layout.rLobby, layout.rOffice, layout.rOffice}));
        guards.Add(new Guard(layout.rSecurity, new List<Room> { layout.rSecurity }));
        guards.Add(new Guard(layout.rLanding, new List<Room> { layout.rLanding }));
        guards.Add(new Guard(layout.rManager, new List<Room> { layout.rManager, layout.rManager, layout.rServer, layout.rCounting, layout.rCounting }));
        guards.Add(new Guard(layout.rSafeDep, new List<Room> { layout.rSafeDep }));
        guards.Add(new Guard(layout.rBoard, new List<Room> { layout.rBoard, layout.rBoard, layout.rBreak, layout.rBreak }));
    }

    public void UpdateGuardPos(int turn)
    {
        foreach (Guard g in guards) { g.Move(turn); }
    }

    public string NextGuardPos(Room r, int turn)
    {
        foreach (Guard g in guards)
        {
            if (r == g.NextPos(turn)) { return " [GUARD NEXT]"; }
        }
        return "";
    }

    public void GuardCheck(Player player)
    {
        foreach (Guard g in guards) { g.CheckRoom(player); }
    }
}


class Display
{
    public string Render(Bank bank, GameState state, Layout layout, Player player, Bag bag)
    {
        string take = player.Inventory().Take(layout.rStreet, bag);
        string canTake = ""; string drop = ""; string use = ""; string end = "";
        int disguisesLeft = player.GetTurnDisguised() + 2 - bank.Turn();

        if (player.Location().GetItemList().Count != 0) { canTake = ", [T]ake item"; }
        if (bank.Turn() > 2 && player.Location() == layout.rStreet) { end = ", [E]nd heist"; }
        foreach (Item i in player.Inventory().GetInventory())
        {
            if (i is not EmptySlot) { drop = ", [D]rop item"; use = ", [U]se item"; break; }
        }

        Console.Clear();
        string output = "";
        output += $"{new string('=', 57)}\n  THE BANK{new string(' ', 37 - take.Length)}TAKE : £ {take}\n{new string('=', 57)}";
        output += $"\n  {player.Location().ToString()}\n";
        output += $"\n  Items: {player.Location().GetItems()}\n";
        output += $"\n  Doors:\n{player.Location().GetDoors(bank)}\n";
        output += $"{new string('-', 57)}\n  {player.Inventory().ToString()}{player.IsDisguised($"\n  [DISGUISED] - {disguisesLeft} turns left")}\n{new string('-', 57)}\n";
        output += $"\nActions: [W]alk{canTake}{drop}{use}{end}\n";
        return output;
    }
}


class Bank
{
    private Player player;
    private GameState state;
    private Layout layout;
    private Display display;
    public Bag bag = new Bag();
    public int turn = 1;
    private string vaultCode = Utils.GenerateCode();

    public Bank()
    {
        bag.Add(new Disguise());
        layout = new Layout(vaultCode, bag);
        state = new GameState(layout);
        display = new Display();
        player = new Player(layout.rStreet, false);
        state.UpdateGuardPos(turn);
    }

    public Room Street() { return layout.rStreet; }
    public int Turn() { return turn; }
    public string NextGuardPos(Room r) { return state.NextGuardPos(r, turn); }
    public string Render(Display display) { return display.Render(this, state, layout, player, bag); }
    public void PlayTurn()
    {
        Console.WriteLine(turn);
        state.UpdateGuardPos(turn);
        Console.WriteLine(Render(display));
        state.GuardCheck(player);
        player.Action(this, display);
        if (turn - 2 == player.GetTurnDisguised()) { player.RemoveDisguise(); }
        turn++;
    }
}


abstract class Entity
{
    protected Room pos;
    
    public Entity(Room pos_)
    {
        pos = pos_;
    }

    public Room Location()
    {
        return pos;
    }

}

class Guard : Entity
{
    private List<Room> path;
    public Guard(Room pos_,  List<Room> path_) : base(pos_)
    {
        path = path_;
        pos.GetGuards().Add(this);
    }

    public bool Move(int turn)
    {
        pos.GetGuards().Remove(this);
        pos = path[turn % path.Count];
        pos.GetGuards().Add(this);
        return true;
    }

    public Room NextPos(int turn)
    {
        Room nextRoom = path[(turn + 1) % path.Count];
        return nextRoom;
    }

    public void CheckRoom(Player player)
    {
        if(player.IsDisguised() == false)
        {
            if (pos == player.Location() && pos.IsRestricted() == true){ CatchPlayer(); }
        }
        else { return; }
    }

    public void CatchPlayer ()
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n{new string(' ', 9)}A guard found you in a restricted area\n");
        Console.Write($"{new string(' ', 22)}**GAME OVER**");
        Console.ReadKey();
        Environment.Exit(0);
    }

}

class Player : Entity
{
    private bool disguised;
    private Inventory inventory;
    private int turnDisguised;
    
    public Player(Room pos_, bool disguised_) : base (pos_)
    {
        inventory = new Inventory();
        disguised = disguised_;
    }
    
    public bool IsDisguised() { return disguised; }
    public string IsDisguised(string s)
    {
        if (disguised == true){ return s; }
        else { return ""; }
    }
    public void Disguise() { disguised = true; }
    public void RemoveDisguise() { disguised = false; }
    public int SetTurnDisguised(int x) { return turnDisguised = x; }
    public int GetTurnDisguised() { return turnDisguised; }

    public Inventory Inventory() { return inventory; }

    public void Action(Bank bank, Display display)
    {
        while (true)
        {
            Console.Write("Enter Action: ");
            string input = Console.ReadLine()!;
            Utils.Return();

            switch (input.ToLower())
            {
                case "w":
                    if (Move() == true) { return; }
                    Console.WriteLine(bank.Render(display));
                    break;
                case "t":
                    TakeItem();
                    Console.WriteLine(bank.Render(display));
                    break;
                case "d":
                    DropItem();
                    Console.WriteLine(bank.Render(display));
                    break;
                case "u":
                    UseItem(bank);         
                    Console.WriteLine(bank.Render(display));        
                    break;
                case "e":
                    if(bank.Turn() > 2 && Location() == bank.Street())
                    {
                        EndHeist(bank);
                        return;
                    }
                    else if (bank.Turn() < 2) { Utils.ErrorMessage("You must end the bank before ending the heist"); break; }
                    else { Utils.ErrorMessage("You must be on the street to end the heist"); break; }

                case "quit":
                    Environment.Exit(0);
                    return;
                default:
                    Console.Write("Unknown action, try again: ");
                    break;
            }
        }
    }

    public bool Move()
    {
        Room temp = pos;
        int choice;
        List<Room> connectedRooms = Location().ConnectedRooms();

        choice = Utils.Select("room", "move to", connectedRooms.Count());
        pos = pos.UseDoor(choice);
        if (temp != pos) { return true; }
        return false;
    }

   public void TakeItem()
    {
        int numItems = Location().GetItemList().Count();
        if (numItems == 0)
        {
            Utils.ErrorMessage("Room is empty");
            return;
        }

        int item = Utils.Select("item", "pick up", numItems);

        char loc = Utils.InventoryOption("Store to", inventory);
        switch (loc)
        {
            case 'b':
                Bag b = Utils.BagPos(inventory);
                if (b.Add(Location().GetItemList()[item]))
                { Location().GetItemList().RemoveAt(item); }
                return;

            case 'i':
                if (inventory.Add(Location().GetItemList()[item]))
                { Location().GetItemList().RemoveAt(item); }
                return;

        }
    }

    public void DropItem()
    {
        int slot;
        Item temp;
        char loc = Utils.InventoryOption("Drop from", inventory);
        switch (loc)
        {
            case 'b':
                Bag b = Utils.BagPos(inventory);
                slot = Utils.Select("item", "drop", b.GetBag().Length);
                temp = b.GetSlot(slot);
                if (b.Drop(slot)) { Location().GetItemList().Add(temp); }
                return;

            case 'i':
                slot = Utils.Select("item", "drop", inventory.GetInventory().Length);
                temp = inventory.GetSlot(slot);
                if (inventory.Drop(slot)) { Location().GetItemList().Add(temp); }
                return;
        }
    }

    public void UseItem(Bank bank)
    {
        int slot;
        char loc = Utils.InventoryOption("Use an item from", inventory);

        switch (loc)
        {
            case 'b':
                Bag b = Utils.BagPos(inventory);
                slot = Utils.Select("item", "use", b.GetBag().Length);
                b.GetSlot(slot).Use(this, bank);
                if (b.GetSlot(slot).GetUses() == 0) { b.ClearSlot(slot); }
                return;
            case 'i':
                slot = Utils.Select("item", "use", inventory.GetInventory().Length);
                inventory.GetSlot(slot).Use(this, bank);
                if (inventory.GetSlot(slot).GetUses() == 0) { inventory.ClearSlot(slot); }
                return;                
        }

    }

    public void EndHeist(Bank Bank)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        string msg = $"You escaped with £ {inventory.Take(Bank.Street(), Bank.bag)}";
        int len = (57 - msg.Length)/2;
        Console.WriteLine($"\n{new string(' ', len)}{msg}\n");
        Console.WriteLine($"{new string(' ', 23)}**YOU WIN**");
        Console.ReadKey();
        Environment.Exit(0);
    }

}

//Items and inventory:

class Inventory
{
    private Item[] inventory;
    public Inventory()
    {
        inventory = new Item[] { new EmptySlot(), new EmptySlot() } ;
    }

    public Item[] GetInventory() { return inventory; }
    public Item GetSlot(int x) { return inventory[x]; }
    public void ClearSlot(int x) { inventory[x] = new EmptySlot(); }

    public bool Add(Item i)
    {
        for (int x = 0; x < 2; x++)
        {
            if (inventory[x] is EmptySlot)
            {
                inventory[x] = i;
                return true;
            }
        }
        Utils.ErrorMessage("There are no free inventory slots");
        return false;
    }

    public bool Drop(int i)
    {
        if (inventory[i] is EmptySlot)
        {
            Utils.ErrorMessage("Slot is already empty");
            return false;
        }
        else
        {
            inventory[i] = new EmptySlot();
            return true;
        }
    }

    public string Take(Room street, Bag bag)
    {
        double take = 0.0;
        foreach(Item i in inventory) { take += i.GetValue(); }
        if (Utils.HasBag(this) == true)
        {
            Bag b = Utils.BagPos(this);
            foreach(Item i in b.GetBag()) { take += i.GetValue(); }
        }        
        
        foreach (Item i in street.GetItemList())
        {
            take += i.GetValue();
            if (i is Bag)
            {
                foreach (Item x in bag.GetBag()) { take += x.GetValue(); }
            }
        }
        
        if (take > 1000.00)
        {
            take /= 1000;
            return (take.ToString() + "K");
        }

        return take.ToString();
    }

    public override string ToString()
    {
        string output = "";
        bool hasbag = false;
        output += "Inventory:";
        foreach (Item i in inventory)
        {
            output += $" [{i.ToString()}]";
        }
        hasbag = Utils.HasBag(this);
        if (hasbag == true)
        {
            Bag b = Utils.BagPos(this);
            output += $"\n  Bag:      {b.Contents()}";
        }
        return output;
    }
}


abstract class Item
{
    protected string name;
    protected int value;
    protected bool fitsBag;
    protected int uses;
    
    public Item(string name_, int value_, bool fitsBag_, int uses_)
    {
        name = name_;
        value = value_;
        fitsBag = fitsBag_;
        uses = uses_;
    }

    public int GetValue() { return value; }
    public bool GetFitsBag() { return fitsBag; }
    public int GetUses() { return uses; }


    public virtual void Use(Player player, Bank Bank)
    {
        Utils.ErrorMessage("Item has no use");
        return;
    }

    public override string ToString()
    {
        return name;
    }
}


class EmptySlot : Item
{
    public EmptySlot() : base("-", 0, true, -1) {}
}


class Bag : Item
{
    private Item[] bag = new Item[6];
    public Bag() : base("Bag", 0, false, -1)
    {
        for (int i = 0; i < 6; i++)
        {
            bag[i] = new EmptySlot();
        }
    }

    public Item[] GetBag() { return bag; }
    public Item GetSlot(int x) { return bag[x]; }
    public void ClearSlot(int x) { bag[x] = new EmptySlot(); }

    public bool Add(Item i)
    {
        if (i.GetFitsBag() == true)
        {
            for (int x = 0; x < 6; x++)
            {
                if (bag[x] is EmptySlot)
                {
                    bag[x] = i;
                    return true;
                }
            }
        }
        else if (i.GetFitsBag() == false)
        {
            Utils.ErrorMessage($"{i.ToString()} does not fit in bag");
            return false;
        }
        Utils.ErrorMessage("There are no free slots in the bag");
        return false;
    }

    public bool Drop(int i)
    {
        if (bag[i] is EmptySlot)
            {
                Utils.ErrorMessage("Slot is already empty");
                return false;
            }
            else
            {
                bag[i] = new EmptySlot();
                return true;
            }
    }

    public string Contents()
    {
        string output = "";
        int x = 1;
        foreach (Item i in bag)
        {
            output += $" [{i.ToString()}]";
            if (x % 3 == 0 && x < 4 )
            {
                output += "\n            ";
            }
            x++;
        }
        return output;
    }
}

class Lockpick : Item
{
    public Lockpick() : base("Lockpick", 0, true, 2) {}

    public override void Use(Player player, Bank Bank)
    {
        Room r = player.Location();
        
        List<Door> lockedDoors = new List<Door>();
        foreach (Door d in r.GetDoors())
        {
            if (d.CheckLocked() == true) { lockedDoors.Add(d); }
        }

        if(lockedDoors.Count > 0)
        {
            int doorNum = Utils.Select("door", "unlock", r.GetDoors().Count());
            if (r.GetDoors()[doorNum] is VaultDoor)
            {
                Utils.ErrorMessage("A code is needed to open the vault");
                return;
            }
            if (r.GetDoors()[doorNum].CheckLocked() == false)
            {
                Utils.ErrorMessage("Door is already unlocked");
                return;
            }
            r.GetDoors()[doorNum].Unlock();
            uses -= 1;
            return;        
        }

        Utils.ErrorMessage("There are no locked doors in this room");
        return;
    }
}

class Disguise : Item
{
    public Disguise() : base("Guard Uniform", 0, true, 1) {}

    public override void Use(Player player, Bank bank)
    {
        if (bank.Turn() == player.GetTurnDisguised() + 2)
        {
            player.Disguise();
            uses -= 1;
            player.SetTurnDisguised(bank.Turn());
            return;
        }
        if (player.IsDisguised() == true)
        {
            Utils.ErrorMessage("You are already disguised");
        }
        else
        {
            player.Disguise();
            uses -= 1;
            player.SetTurnDisguised(bank.Turn());
        }
    }

}

class VaultCode : Item
{
    public VaultCode(string code_) : base($"Vault code: {code_}", 0, true, -1) {}
}

class Cash : Item
{
    public Cash() : base("Cash", 100, true, -1) {}
}

class Necklace : Item
{
    public Necklace() : base("Necklace", 500, true, -1) {}
}

class Gold : Item
{
    public Gold() : base("Gold Bar", 1000, true, -1) {}
}

class Watch : Item
{
    public Watch() : base("Watch", 250, true, -1) {}
}

class HardDrive : Item
{
    public HardDrive() : base("Hard Drive", 1500, true, -1) {}
}

class Painting : Item
{
    public Painting() : base("Painting", 7500, false, -1) {}
}

class Bonds : Item
{
    public Bonds() : base("Bonds", 1000, true, -1) {}
}

class Copper : Item
{
    public Copper() : base("Copper wire", 10, true, -1) {}
}

//To build rooms:

class Room
{
    private List<Door> Doors = new List<Door>();
    private List<Item> Items = new List<Item>();
    private List<Guard> Guards = new List<Guard>();
    private string roomName = "";
    private bool restricted;

    public Room(string roomName_, bool restricted_, List<Item> Items_)
    {
        roomName = roomName_;
        restricted = restricted_;
        Items = Items_; 
    }

    public string GetName() { return roomName; }
    public List<Door> GetDoors() { return Doors; }
    public List<Item> GetItemList() { return Items; }
    public List<Guard> GetGuards() { return Guards; }
    public string IsRestricted(string s)
    {
        if (restricted == true) { return s; }
        else { return "";}
    }
    public bool IsRestricted() { return restricted; }

    public bool HasGuards()
    {
        if (Guards.Count != 0) { return true; }
        else { return false; }
    }

    public string HasGuards(string s)
    {
        if (HasGuards() == true) { return s; }
        else { return ""; }
    }

    public void CreateDoor(Room r, bool isLocked, bool reverse = true)
    {
        Door d = new RoomDoor(this, r, isLocked);
        Doors.Add(d);
        if (reverse) { r.CreateDoor(this, d.CheckLocked(), false); }
    }

    public void CreateVaultDoor(Room r, bool islocked, string code, bool reverse = true)
    {
        VaultDoor d = new VaultDoor(this, r, islocked, code);
        Doors.Add(d);
        if (reverse) { r.CreateVaultDoor(this, d.CheckLocked(), code, false); }
    }

    public Room UseDoor(int d)
    {
        if (Doors[d].CheckLocked() == true)
        {
            if (Doors[d] is not VaultDoor)
            {
                Utils.ErrorMessage("Room is locked");
                return this;
            }
            else
            {
                VaultDoor v = (VaultDoor)Doors[d];
                v.EnterCode();
                return this;
            }
        }

        else { return Doors[d].UseDoor(this); }
    }

    public string GetDoors(Bank bank)
    {
        string output = "";
        string locked;
        
        int i = 1;
        foreach (Door d in Doors)
        {
            Room r = d.UseDoor(this);

            string guardNext = "";
            guardNext = bank.NextGuardPos(r);
            if(d.CheckLocked()) { locked = $" : Locked  "; }
            else { locked = $" : Unlocked"; }

            int len = 35 - r.GetName().Length - locked.Length;

            output += $"    {i.ToString()} - {r.GetName()}";
            output += $"{new string(' ', len)}{locked}";
            output += $"{guardNext}\n";

            i++;
        }
        return output;
    }
    
    public List<Room> ConnectedRooms()
    {
        List<Room> joinedRooms = new List<Room>();
        foreach (Door d in Doors)
        {
            joinedRooms.Add(d.UseDoor(this));
        }
        return joinedRooms;
    }

    public string GetItems()
    {
        string output = "";
        if (Items.Count == 0)
        {
            output += "None";
            return output;
        }
        int x = 1;
        foreach (Item i in Items)
        {
            if (x % 5 == 0) { output += "\n         "; }
            output += $"[{i.ToString()}] ";
            x++;
        }
        return output;
    }

    public override string ToString()
    {
        string output = roomName.ToUpper();
        string guards = "";
        string restricted = "";

        if (HasGuards() == true) { guards = "[GUARD PRESENT]"; }
        if (IsRestricted() == true) { restricted = "  [RESTRICTED AREA]"; }
        int len = 54 - roomName.Length - guards.Length - restricted.Length;
        output += new string(' ', len) + guards + restricted;

        return output;
    }
}


abstract class Door
{
    protected bool locked;
    protected Room r1;
    protected Room r2;

    public Door(Room r1_, Room r2_, bool locked_)
    {
        r1 = r1_;
        r2 = r2_;
        locked = locked_;
    }

    public Room UseDoor(Room r)
    {
        
        if (r == r1) { return r2; }
        else { return r1; }
    }

    public bool CheckLocked()
    {
        return locked;
    }

    public void Unlock()
    {
        locked = false;
        foreach(Door d in r2.GetDoors())
        {
            if (d.UseDoor(r2) == r1)
            {
                d.locked = false;
                break;
            }
        }
    }
}

class RoomDoor : Door
{
    public RoomDoor(Room r1_, Room r2_, bool locked_) : base(r1_, r2_, locked_) { }
}

class VaultDoor : Door
{
    private string code;
    public VaultDoor(Room r1_, Room r2_, bool locked_, string code_) : base(r1_, r2_, locked_)
    {
        code = code_;
    }

    public void EnterCode()
    {
        Console.Write("Enter the code: ");
        string input = Console.ReadLine()!;
        Utils.Return();

        if (input == code)
        {
            this.Unlock();
        }
        else
        {
            Utils.ErrorMessage("Incorrect code");
        }
        return;
    }
}


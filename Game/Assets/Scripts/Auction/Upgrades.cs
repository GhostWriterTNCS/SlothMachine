public struct Upgrade {
	public string name;
	public UpgradeTypes type;
	public string description;

	public Upgrade(string name, UpgradeTypes type, string description) {
		this.name = name;
		this.type = type;
		this.description = description;
	}
}

public enum UpgradeTypes {
	Hands,
	Feet,
	Armor,
	Core
}
public class Upgrades {
	public static Upgrade[][] list = {
		new Upgrade[] { }, // Upgrade levels start from 1.
		new Upgrade[] {
			new Upgrade("",UpgradeTypes.Core, ""), // Upgrade IDs start from 1.
			new Upgrade("Hammer", UpgradeTypes.Hands, "A light hammer."),
			new Upgrade("Spike ball", UpgradeTypes.Feet, "A spiked ball.")
		},
		new Upgrade[] {
			new Upgrade("", UpgradeTypes.Core, ""), // Upgrade IDs start from 1.
			new Upgrade("Spike armor", UpgradeTypes.Armor, "A spiked armor."),
			new Upgrade("Core Mk 2", UpgradeTypes.Core, "A more powerful core.")
		}
	};
}

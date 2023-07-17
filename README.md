# ChainSafeTakeHomeProject

## Requirements

This Project uses

- Unity 2021.3.15f1 LTS, other 2021 LTS versions should be fine. However versions of early 2021.3 or earlier will not run properly as this project uses NetCode For Gameobjects
- NetCode for GameObjects and Multiplay for networking
- Playfab for user and inventory management
- Custom plugins of my own https://github.com/rob1997/Core and https://github.com/rob1997/UI (Reduced version since they're bulky)
- Unity Addressables for asset loading
- Newtonsoft's Json.Net for serializing and Deserializing

## How to Run

- This Project needs an internet connection to run since it's constantly communicating with a server
- To start runing from editor simply load `0_Loading` and press play, you might need to build addressables in `Window -> Asset Management -> Addressables - Groups`
- Preferable resolution is 1920 x 1080 but UI is responsive and visible in any resolution
-  Once project is running to create an account you can simply login with a new account and it'll created
-  The `Keep me signed in` option is enabled by default however consider turning it off when running multiple builds on the same machine for networking
-  Once logged in you'll be greeted with Inventory and Store User Interfaces where you can buy Equip and Unequip any of the items
-  There's 4 categories of items (Hat, Shirt, Pants and Shoes) with 4 items in each category
-  You are granted an intial of 500 virtual currency upon first login, you can use that to buy and equip any of the items
-  All changes to inventory and slots are saved on a server for the specific user, saved configuration can be loaded from any other instance
-  you can use testUser and testUser1 for already populated accounts


### Multiplayer

- Once you have equipped all item you want you can press Start at the top to join a Game Instance **!!!IMPORTANT!!!** if there's no already existing session or host you'll have to tick the Is Host toggle at the top before pressing Start, now you should see a the Game Instance running with you in it.
- **!!!IMPORTANT!!!** You can join an already existing session only if the Host instace is on the same machine (currently IP is only configured for local networking testing) this isn't the case for Inventory and Slots changes, they're reflected on any instance on other machines
- You can run multiple builds with different users joining a game instance on the same machine

**Enjoy**

╔════════════╗</br>
║────────────║</br>
║─╦╩╩═╩╩══╗──║</br>
║─╣─╔═══╗─║──║</br>
║─║─╚═══╝─╚╗─║</br>
║─║─╔════╗─║─║</br>
║─╣─╚════╝─║─║</br>
║─╩╦╦═╦╦═══╝─║</br>
║────────────║</br>
╚════════════╝</br>

When adding a new upgrade:

Add prefab to scroll area
Create file Upgrades/[new name]Manager.cs
Fill it in like the other managers
Update UpgradeManager with this new Upgrades/[new name]Manager.cs
Drag new script to UpdateManager
Add button click event to added prefab, call OnBuy[new name]

flow:

buy button is enabled if enough money
    UpgradeManager.Update
    UpgradeManager.UpdateUpgradeUiButtons
    UpdateUI for each [name]Manager
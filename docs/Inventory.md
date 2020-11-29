# Player Inventory

游戏内的背包系统

Full Name: `Prototype.Inventory.PlayerInventory`

## PlayerInventory

`PlayerInventory` 是游戏玩家的整个背包，背包内有若干个 `Inventory` 容器，：
- `Pixels` 保存玩家拥有的所有像素素材
- `Weapons` 保存玩家所有的像素武器

## Inventory

`Inventory` 是保存物品的容器，物品在 `Inventory` 内可以堆叠，用一个 `ItemGroup` 对象来表示一个堆叠的物品，

可以通过访问 `Inventory.ItemGroups` 遍历容器内所有堆叠的物品，但是对物品的增删必须通过其成员函数 `SaveItem` 和 `Take` 完成。

## ItemGroup

`ItemGroup` 表示堆叠在一起的同类物品，其属性 `ItemType` 指定了这堆物品的类型。通过 `Add` 和 `Take` 可以放入或拿取一个物品实例 `Item`

## ItemType

`ItemType` 表示一种物品，例如某种火属性的像素素材。对于不可堆叠的物品，可以通过给每个物品实例设置一个独立的 `ItemType` 实例实现。

`ItemType` 是一个 `ScriptableObject`，对于不同的物品，应当创建对应的 Asset 文件并保存在项目里。

可以通过派生类给 `ItemType` 赋予更多的属性，例如目前的 `PixelType`。

由于像素画 `PixelImage` 物品不可堆叠，其 `ItemType` 在实例化时由内部创建。

## Item

一个 `Item` 实例就是一个物品实例。
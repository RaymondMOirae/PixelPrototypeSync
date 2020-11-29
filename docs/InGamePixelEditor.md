# Pixel Editor in Game

游戏内的像素图编辑器 UI。

## Usage

`PixelEditor` 的使用需要传入一个玩家背包 `PlayerInventory` 实例，编辑器将使用玩家背包中所拥有的所有像素素材作为调色板，并且允许玩家选择背包内的像素画进行编辑和保存。

```c#

async void Test()
{
    await PixelEditor.Instance.Edit(playerInventory);
}

```

`PixelEditor.Instance.Edit` 是一个异步函数，在玩家完成编辑并关闭编辑器后完成函数调用。 

玩家对武器的修改和编辑将直接应用到背包里对应的武器上，并且背包里的像素素材可能会被修改（使用掉）。

在玩家完成编辑后，需要手动更新武器伤害数据和渲染组件。

可以参考 `Assets/Script/Test/TestPixelEditor.cs` 中的用例。
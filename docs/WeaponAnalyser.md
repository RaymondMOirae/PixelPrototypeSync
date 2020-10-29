# Weapon Analyser

用于从给定的像素图中分析得出该像素武器的伤害数据。

需要为每个不同的图像创建不同的分析器，分析器可以反复更新分析数据。

## API

### Constructor

#### public PixelWeaponAnalyser(PixelImage image, WeaponForwardDirection forwardCorner)

Parameter | Description
----------|------------
`image` | 待分析的像素图像
`forwardCorner` | 武器朝向的方向，目前仅支持 `TopLeft`


### Methods

#### public void UpdateWeaponData()

重新分析当前图像的武器数据。



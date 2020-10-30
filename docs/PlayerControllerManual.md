# Player Controller Manual

## 移动相关参数

### Move Speed

- **类型：**Vector2
- **含义：**
  - X：角色横向移动的速度
  - Y：角色纵向移动的速度
- **其他：**

### Turning Threshold Angle

- **类型：**float
- **含义：**若摇杆输入的方向与当前人物朝向夹角大于该值，则转向到摇杆所在范围代表的（上下左右）方向
- **其他：**请不要取45°和90°这两个极值，前者可能导致转向鬼畜，后者会导致无法转向

## 攻击相关参数

<img src="D:\EDEN\Code\UnityProject\PixelPrototype\docs\res\PlayerGizmos.png" alt="PlayerGizmos"  />

### Attack Radius

- **类型：**float
- **含义：**玩家的最大攻击半径，即图中的长度***l***

### Mid Outer Angle

- **类型：**float
- **含义：**图中**∠1**对应的度数，大小为“前方攻击”范围张角的一半
- **其他：**该值非负，否则导致攻击判断错误

### Side Outer Angle

- **类型：**float
- **含义：**图中**∠2**对应的数独，请注意**∠2 = ∠1 + “左/右攻击的张角”**
- **其他：**该值非负，否则导致攻击判断错误

### Attack Layer

- **类型：**LayerMask
- **含义：**让玩家只攻击Layer设置为“AttackableUnits”的游戏物体。请保持其为“AttackableUnits”
- **其他：**

### Cur Dir

- **类型：**Vector2
- **含义：**当前玩家的朝向，即图中蓝色射线所指方向
- **其他：**请不要手动调整该值，其仅用于检视角色当前朝向
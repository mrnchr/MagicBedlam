> #### **Споры о стиле бессмысленны. Должно быть руководство по стилю, и вы должны следовать ему.**
> *Ребекка Мерфи*

*Это адаптированное руководство по стилю, опирающиеся на [этот источник](https://github.com/justinwasilenko/Unity-Style-Guide#arguments-over-style-are-pointless-there-should-be-a-style-guide-and-you-should-follow-it)*

Если вы заметили, что кто-то не следует руководству по стилю, скажите ему об этом и не позволяйте плодить хаос в проекте.

## 1. Структура проекта
Ассеты распределены по папкам по контенту. Пример иерархии папок:
<pre>
Assets
    <a name="#structure-developers">_Developers</a>(Use a `_`to keep this folder at the top)
        DeveloperName
            (Work in progress assets)
    <a name="structure-top-level">ProjectName</a>
        Characters
            Anakin
        FX
            Vehicles
                Abilities
					IonCannon
						(Particle Systems, Textures)
			Weapons
		Gameplay
			Characters
			Equipment
			Input
			Vehicles
				Abilities
				Air
					TieFighter
						(Models, Textures, Materials, Prefabs)
		<a name="#structure-levels">Levels</a>
			Frontend
			Act1
				Level1
		Lighting
			HDRI
			Lut
			Textures
		MaterialLibrary
			Debug
			Shaders
		Objects
			Architecture (Single use big objects)
				DeathStar
			Props (Repeating objects to fill a level)
				ObjectSets
					DeathStar
		Scripts
			AI
			Gameplay
				Input
			Tools
		Sound
			Characters
			Vehicles
				TieFighter
					Abilities
						Afterburners
			Weapons
		UI
			Art
				Buttons
			Resources
				Fonts
    ExpansionPack (DLC)
    Plugins
    ThirdPartySDK  
</pre>

### 1.1 Имена папок
Всегда используйте PascaleCase
Используйте только символы a-z, A-Z и 0-9. Другие символы могут вызывать ошибки или приводить к странному поведению в различных программах и ОС.
Избегайте пустых папок
  
### 1.2 Использование папок верхнего уровня для конкретных ассетов проекта
Все ассеты проекта должны находится в папке своего проекта.

### 1.3 Использование папки Developers для локального тестирования
Папка содержит файлы, необходимые отедльному разработчику для реализации собственных нужд, будь то проверка гипотез, разработка контента или личные интересы.

### 1.4 Все файлы сцен хранятся в папке Levels
### 1.5 Определять права редактирования ассетов
Такие ассеты, как префабы и сцены, сложно поддаются совместному редактированию. Необходимо определять, кто в конкретный момент времени работает с этими ассетами.

### 1.6 Не создавать папки с названиями ассетов или их типов
Избегайте названий Assets, Meshes, Textures или Materials. Они излишни. Имена файлов говорят все про их типы.

### 1.7 Очень большие наборы ассетов получают собственную папку
Это можно рассматривать как исключение правила 1.6. 
Существуют определенные типы активов, которые имеют огромный объем связанных файлов, где каждый ассет имеет уникальное назначение. Наиболее распространнеными являются анимации и аудио. Если их становится больше 15, следует переместить их в собственную папку.

### 1.8 MaterialLibrary
Файлы, невходящие в другие папки, попадают в эту. Например, шейдеры, файлы отладки или общие текстуры шума.

### 1.9 Иерархия сцены
Рука об руку с иерархией проекта идет иерархия сцены.
- Все пустые объекты располагаются в точках (0, 0, 0) с вращением и масштабов по умолчанию.
- Все объекты располагаются в глобальных пустых объектах, имена которых начинаются с трех дефисов и пишутся заглавными буквами (например, ---LANDSCAPE).
- Пустые объекты, неявляющиеся глобальными, начинаются с одного дефиса и называются стандартно.
- Пустые объекты, которые являются контейнерами для скриптов используйте префикс "@" (например, @Network)
- При создании экземпляра объекта из скрипта, обязательно поместите его в _Dynamic.

## 2. Скрипты

### 2.1 Организация класса
Исходные файлы должны содержать только один общедоступный тип, хотя разрешено несколько внутренних классов.
Исходным файлам должно быть присвоено имя общедоступного класса в файле.
Глобальное пространство имен имеет название папки верхнего уровня, в которой находится скрипт.

**Сортировка**
В общем случае члены класса сгруппированы по секциям:
- Константные поля
- Статичные поля
- Поля
- Конструкторы
- Свойства
- События / Делегаты
- Методы жизненного цикла Unity (Awake, OnEnable, OnDisable, OnDestoy)
- Методы
- Вложенные типы

Внутри каждой группы порядок модификаторов доступа:
- public 
- internal
- protected
- private

Поля атрибутами идут в порядке:
- `[Serializable]`
- `[HideInInspector]`

Сериализуемые поля идут в порядке уменьшения вероятности множественных изменений. Данные, влияющие на EditMode помещаются в самый низ инспектора в отдельную группу "EditMode Data". 
Методы жизненного цикла идут в порядке вызова их в Unity.


**Все публичные функции должны иметь summary**
**Группы полей**
Если в классе 5-10 сериализуемых полей их следует объединять в группы с помощью помещения в отдельную структуру.

**Комментарии**
Комментарии опысывают замысел программиста, ход его рассуждений или порядок выполнения алгоритма. 
Стиль комментария:
- Комментарий помещается на отдельной строке, а не в конце строки
- Текст комментария начинается с заглавной буквы
- Между разделителем комментариев (//) и текстом один пробел
- Комментарий размещается над кодом, который поясняет

**Пробелы**
Используйте один пробел после запятой между аргументами функции. 

Пример: `Console.In.Read(myChar, 0, 1);`
- Не используйте пробелы после скобок и аргументов функции.
- Не используйте пробелы между именем функции и круглыми скобками.
- Не используйте пробелы внутри скобок

### 2.2 Компиляция
Все скрипты должны компилироваться без предупреждения и ошибок. Не отправляйте сломанные скрипты в систему контроля версий.

### 2.3 Переменные 
Все имена нелогических переменных должны быть именем существительным
Публичные переменные пишутся в PascalCase. Локальные и защищенные переменные - в camelCase. Приватные переменные - в camelCase с префиксом "_". Для аббревиатур разрешается использовать PascalCase.
Имена переменных не должны дублировать свой контекст или иметь венгерскую или похожие нотации.
По умолчанию следует делать переменные защищенными. Не делайте переменные публичными, пока они вам не понадобятся. Не делайте перменные приватными пока не захотите ограничить к ним доступ из дочерних классов.
Все сериализуемые поля должны иметь описание в `[Tooltip]` полях, объясняющее, как изменение этого значения влияет на поведение скрипта. Поле и его описание имеет связку "is a".
Используйте атрибут диапазона, `[Range(min, max)]`, или OnValidate() функцию.
Все логические переменные должны быть названы с префиксом глагола, например, isDead, hasItem, canJump.
Не исползуйте логические переменные для представления зависимых состояний. Вместо этого используйте перечисления.
Например, не используйте isReloading и isEquipping, если оружие нельзя одновременно перезаряжать и снаряжать.
Перечисления используют PascalCase и используют имена в единственном числе.
Массивы дожны называться существительными во множественном числе.
Интерфейсы начинаются с заглавной буквы I

### 2.4 Функции, события и диспетчеры
Все функции должны быть глаголами.
Функции, возвращающие bool, должны задавать вопрос.
События и диспетчеры должны начинаться с `On`

## 3. Соглашения об именах ассетов
Все ассеты используют PascaleCase

### 3.1 Базовое имя ассета -
`Prefix_BaseAssetName_Variant_Suffix`

`Prefix` и `Suffix` должны определяться типом актива с помощью таблиц ниже.

`BaseAssetName` следует определяться коротким легко узнаваемым названием, относящимся к контексту данной группы активов.
Для уникальных и специфических вариантов активов существует `Variant` - это либо короткое легко узнаваемое имя, либо двузначное число.

<a name="1.1-examples"></a>
#### Примеры

##### Character

| Asset Type               | Asset Name   |
| ------------------------ | ------------ |
| Skeletal Mesh            | SK_Bob       |
| Material                 | M_Bob        |
| Texture (Diffuse/Albedo) | T_Bob_D      |
| Texture (Normal)         | T_Bob_N      |
| Texture (Evil Diffuse)   | T_Bob_Evil_D |

##### Prop

| Asset Type               | Asset Name   |
| ------------------------ | ------------ |
| Static Mesh (01)         | SM_Rock_01   |
| Static Mesh (02)         | SM_Rock_02   |
| Static Mesh (03)         | SM_Rock_03   |
| Material                 | M_Rock       |
| Material Instance (Snow) | MI_Rock_Snow |

<a name="asset-name-modifiers"></a>
### 3.2 Модификаторы имени ассета

При именовании актива используйте эти таблицы для определения префикса и суффикса.

#### Sections

> 3.2.1 [Most Common](#anc-common)

> 3.2.2 [Animations](#anc-animations)

> 3.2.3 [Artificial Intelligence](#anc-ai)

> 3.2.4 [Prefabs](#anc-prefab)

> 3.2.5 [Materials](#anc-materials)

> 3.2.6 [Textures](#anc-textures)

> 3.2.7 [Miscellaneous](#anc-misc)

> 3.2.8 [Physics](#anc-physics)

> 3.2.9 [Audio](#anc-audio)

> 3.2.10 [User Interface](#anc-ui)

> 3.2.11 [Effects](#anc-effects)

<a name="anc-common"></a>
#### Most Common

| Asset Type              | Prefix     | Suffix     | Notes                            |
| ----------------------- | ---------- | ---------- | -------------------------------- |
| Level / Scene           |  *          |            | [Should be in a folder called Levels.](#levels) e.g. `Levels/A4_C17_Parking_Garage.unity` |
| Level (Persistent)      |            | _P         |                                  |
| Level (Audio)           |            | _Audio     |                                  |
| Level (Lighting)        |            | _Lighting  |                                  |
| Level (Geometry)        |            | _Geo       |                                  |
| Level (Gameplay)        |            | _Gameplay  |                                  |
| Prefab                  |        |            |                                  |
| Material                | M_         |            |                                  |
| Static Mesh             | SM_       |            |                                  |
| Skeletal Mesh           | SK_       |            |                                  |
| Texture                 | T_         | _?         | See [Textures](#anc-textures)    |
| Particle System         | PS_       |            |                                  |

<a name="anc-models"></a>

#### 3.2.1a 3D Models (FBX Files)

| Asset Type    | Prefix | Suffix | Notes |
| ------------- | ------ | ------ | ----- |
| Characters    | CH_    |        |       |
| Vehicles      | VH_    |        |       |
| Weapons       | WP_    |        |       |
| Static Mesh   | SM_    |        |       |
| Skeletal Mesh | SK_    |        |       |
| Skeleton      | SKEL_  |        |       |
| Rig           | RIG_   |        |       |

#### 3.2.1b 3d Models (3ds Max)

| Asset Type    | Prefix | Suffix      | Notes                                   |
| ------------- | ------ | ----------- | --------------------------------------- |
| Mesh          |        | _mesh_lod0* | Only use LOD suffix if model uses LOD's |
| Mesh Collider |        | _collider   |                                         |

<a name="anc-animations"></a>

#### 3.2.2 Animations 
| Asset Type           | Prefix | Suffix | Notes |
| -------------------- | ------ | ------ | ----- |
| Animation Clip       | A_     |        |       |
| Animation Controller | AC_    |        |       |
| Avatar Mask          | AM_    |        |       |
| Morph Target         | MT_    |        |       |

<a name="anc-ai"></a>
#### 3.2.3 Artificial Intelligence

| Asset Type              | Prefix     | Suffix     | Notes                            |
| ----------------------- | ---------- | ---------- | -------------------------------- |
| AI Controller           | AIC_     |            |                                  |
| Behavior Tree           | BT_      |            |                                  |
| Blackboard              | BB_       |            |                                  |
| Decorator               | BTDecorator_ |          |                                  |
| Service                 | BTService_ |            |                                  |
| Task                    | BTTask_  |            |                                  |
| Environment Query       | EQS_     |            |                                  |
| EnvQueryContext         | EQS_     | Context    |                                  |

<a name="anc-prefab"></a>
#### 3.2.4 Prefabs

| Asset Type              | Prefix     | Suffix     | Notes                            |
| ----------------------- | ---------- | ---------- | -------------------------------- |
| Prefab         |        |            |                                  |
| Prefab Instance         | I       |            |                                  |
| Scriptable Object       |     |        | Assigned "Blueprint" label in Editor |

<a name="anc-materials"></a>

#### 3.2.5 Materials
| Asset Type        | Prefix | Suffix | Notes |
| ----------------- | ------ | ------ | ----- |
| Material          | M_     |        |       |
| Material Instance | MI_    |        |       |
| Physical Material | PM_    |        |       |

<a name="anc-textures"></a>

#### 3.2.6 Textures
| Asset Type              | Prefix     | Suffix     | Notes                            |
| ----------------------- | ---------- | ---------- | -------------------------------- |
| Texture                 | T_         |            |                                  |
| Texture (Diffuse/Albedo/Base Color)| T_ | _D      |                                  |
| Texture (Normal)        | T_         | _N         |                                  |
| Texture (Roughness)     | T_         | _R         |                                  |
| Texture (Alpha/Opacity) | T_         | _A         |                                  |
| Texture (Ambient Occlusion) | T_     | _AO      |                                  |
| Texture (Bump)          | T_         | _B         |                                  |
| Texture (Emissive)      | T_         | _E         |                                  |
| Texture (Mask)          | T_         | _M         |                                  |
| Texture (Specular)      | T_         | _S         |                                  |
| Texture (Packed)        | T_         | _*         | See notes below about [packing](#anc-textures-packing). |
| Texture Cube            | TC_       |            |                                  |
| Media Texture           | MT_       |            |                                  |
| Render Target           | RT_       |            |                                  |
| Cube Render Target      | RTC_     |            |                                  |
| Texture Light Profile   | TLP_     |            |                                  |

<a name="anc-textures-packing"></a>

#### 3.2.7 Miscellaneous

| Asset Type                      | Prefix | Suffix | Notes |
| ------------------------------- | ------ | ------ | ----- |
| Universal Render Pipeline Asset | URP_   |        |       |
| Post Process Volume Profile     | PP_    |        |       |
| User Interface                  | UI_    |        |       |

<a name="anc-physics"></a>
#### 3.2.8 Physics

| Asset Type        | Prefix | Suffix | Notes |
| ----------------- | ------ | ------ | ----- |
| Physical Material | PM_    |        |       |

<a name="anc-audio"></a>

#### 3.2.9 Audio

| Asset Type     | Prefix | Suffix | Notes                                                        |
| -------------- | ------ | ------ | ------------------------------------------------------------ |
| Audio Clip     | A_     |        |                                                              |
| Audio Mixer    | MIX_   |        |                                                              |
| Dialogue Voice | DV_    |        |                                                              |
| Audio Class    |        |        | No prefix/suffix. Should be put in a folder called AudioClasses |

<a name="anc-ui"></a>
#### 3.2.10 User Interface
| Asset Type       | Prefix | Suffix | Notes |
| ---------------- | ------ | ------ | ----- |
| Font             | Font_  |        |       |
| Texture (Sprite) | T_     | _GUI   |       |

<a name="anc-effects"></a>
#### 3.2.11 Effects
| Asset Type      | Prefix | Suffix | Notes |
| --------------- | ------ | ------ | ----- |
| Particle System | PS_    |        |       |
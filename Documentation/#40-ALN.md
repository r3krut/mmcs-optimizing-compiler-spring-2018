### Название задачи

Тестирование итерационного алгоритма для достигающих выражений

#### Постановка задачи

Провести тестирование алгоритма достигающих выражений

#### Зависимости задач в графе задач

* Базовые блоки
* Трехадресный код
* Граф потока управления
* Поиск множеств `E_GEN` и `E_KILL`
* Итерационный алгоритм поиска достигающих выражений

#### Практическая часть задачи (реализация)

*	Были произведены автоматические тесты для алгоритма поиска достигающих выражений

#### Тесты

Трёхадресный код теста

```TAC
    0)   t0 = 3            |
    1)   t1 = 5 + t0       | B1
    2)   t2 = t1 + t0      |
    
    3)   t3 = 4 + t2       |
    4)   t1 = 10           | B2
    5)   if (1) goto 3)    |
    
    6)   t4 = t1 + 5       |
    7)   t5 = t3 + t0      |
    8)   t0 = 100          | B3
    9)   if (2) goto 6)    |
    
    10)  t6 = t5 + 10      |
    11)  t7 = t6 + 10      |
    12)  t8 = t6 + t7      | B4
    13)  t6 = 3            |
    14)  t5 = 100          |
```

Граф потока упраления трёхадресного кода

```CFG               
             B1
             |
             |   ____
             v  /    \
            B2-v______|
             |
             |   ____
             v  /    \
            B3-v______|
             |   
             |
             v
             B4
```

#### Пример работы

```CFG               
	B1
	-------------
	e_gen = {l1, l2}, e_kill = {l3, l6, l7}
	IN = {}, OUT = {l1, l2}
	
	B2
	-------------
	e_gen = {l3}, e_kill = {l2, l6, l7}
	IN = {l1, l2}, OUT = {l1, l3}
	
	B3
	-------------
	e_gen = {l6}, e_kill = {l1, l2, l7, l10}
	IN = {l1, l3}, OUT = {l3, l6}
	
	B4
	-------------
	e_gen = {}, e_kill = {l10, l11, l12}
	IN = {l3, l6}, OUT = {l3, l6}
```
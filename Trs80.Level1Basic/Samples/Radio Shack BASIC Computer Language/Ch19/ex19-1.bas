1 in. "Enter a number from 1 to 100";n
2 f.i=1ton;j=rnd(32767);n.i
10 rem * craps game *
20 cls
30 gosub 300:p=n
40 p.:p."You rolled ****";a;" and ";b;"****"
50 on p goto 60, 120, 120, 100, 100, 100, 110, 100, 100, 100, 110, 120
60 rem * used for the on statement if p=1 (which it can't)*
100 p."Your point is";n:goto 130
110 print "You win!":p.:end
120 print "You lose.":p.:end
130 gosub 300:m=n
135 p.:p."You rolled ****";a;" and ";b;"****"
140 if p=m then 110
150 if m=7 then 120
160 g.130
300 a=rnd(6):b=rnd(6):n=a+b
310 return

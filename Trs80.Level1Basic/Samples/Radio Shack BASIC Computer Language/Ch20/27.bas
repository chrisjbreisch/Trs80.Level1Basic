10 CLS
20 X=64
30 FOR K=0 TO 7
40   SET(X+K, 40+K)
50   SET(X-K, 40+K)
60 NEXT K
70 FOR K=0 TO 5
80   SET(X+K,34+K)
90   SET(X,34+K)
100  SET(X-K,34+K)
110 NEXT K
999 GOTO 999
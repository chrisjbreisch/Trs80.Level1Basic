10 INPUT " STARTING HORIZONTAL BLOCK (0 TO 127)";H
20 INPUT " ENDING HORIZONTAL BLOCK (0 TO 127)";I
30 INPUT " STARTING VERTICAL BLOCK (0 TO 47)";V
40 INPUT " ENDING VERTICAL BLOCK (0 TO 47)";W
50 CLS
60 FOR X = H TO I
70    FOR Y = V TO W
80       SET(X,Y)
90    NEXT Y
100 NEXT X
999 GOTO 999
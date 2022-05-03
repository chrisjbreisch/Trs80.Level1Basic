10 CLS
20 PRINT AT 407, "H  M   S"
30 FOR H = 0 TO 23
40    FOR M = 0 TO 59
50       FOR S = 0 TO 59
60          PRINT AT 470, H;":";M;":";S;" "
70          FOR N = 1 TO 2000000: NEXT N
80       NEXT S
90    NEXT M
100 NEXT H
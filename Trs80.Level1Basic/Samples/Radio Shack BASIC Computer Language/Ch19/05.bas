10 H=0:T=0:P.
20 IN."HOW MANY TIMES SHALL WE FLIP THE COIN";F:CLS
30 P."YOU STAND BY WHILE I DO THE FLIPPING - - - -"
40 F.N=1TOF:X=RND(2):ONXG. 60, 70
50 P."IT BOMBED. WAS NEITHER A 1 NOR A 2. X IS";X:END
60 H=H+1:G.80
70 T=T+1
80 N.N:P.:P.:P.:P.
90 P."HEADS","TAILS","TOTAL FLIPS":P.:P.H,T,F
100 P.100*H/F;"%",100*T/F;"%":P.:P.:P.
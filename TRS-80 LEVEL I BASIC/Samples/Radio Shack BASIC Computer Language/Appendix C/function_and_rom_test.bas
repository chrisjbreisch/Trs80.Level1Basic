10 in. "Type 1, then press <Enter>";x
15 cls:p.at0; "TRS-80 FUNCTION TEST"
20 read y
30 data 2
40 restore
50 read y
55 f.a=1to1000;n.a
60 ifx>ystop
70 ifx>=ystop
80 ify<xstop
90 ify<=xstop
100 f.x=1to10step2
110 goto130
120 stop
130 gos.150
140 goto160
150 return
160 onxgoto180
170 stop
180 set(x,y)
185 ifpoint(x,y)g.190
187 stop
190 reset(x,y)
200 ifx<>y-1stop
210 ify=x+1g.230
220 stop
230 z=rnd(0)
240 x=1.1:x=int(x)
245 y=abs(x)/2+.5
250 ify=1g.270
260 stop
270 rem everything is ok
290 cls:p.tab(5),"ALL FUNCTIONS ARE O.K.,THE RAM TEST IS RUNNING."
300 a=m./4-1:b=0
310 f.y=1to8:q=.5
320 f.b=1toy:q=q*2:n.b
330 f.x=0toa:a(x)=q:n.x
340 f.x=0toa:ifa(x)<>qp. "RAM ERROR":stop
350 n.x
360 p.at68,q:n.y:p.at0;"THE RAM TEST IS COMPLETE."
370 f.a=1to2500:n.a
400 cls:k=1
410 a$=GH
420 f.x=1to32:p.a$;:n.x
430 f.x=1to14:p.t.(29);a$:n.x
440 p.at 469;
450 f.x=1to9:p.a$;:n.x:p.
460 p.t.(21);:f.x=1to9:p.a$;:n.x
470 p.at960;
480 f.x=1to31:p.a$;:n.x
490 in.b$
500 ifk>0a$=80
510 ifk<0a$=GH
520 k=-k
530 cls:g.420

10 rem * demonstration of graphics 'point' statement *
20 p=15:l=119
30 cls
40 p.at5,"This is a demonstration of the point statement---";
50 p.at56,"X     Y";
100 f.i=1to p:set(rnd(113),rnd(45)+2):n.i
110 f.x=0 to 111;f.y=0 to 47
120    if point(x,y) = 0 goto 160
130    p.at l,x;:p.at l+4,y;
140    l=l+64
150    g.170
160    set(x,y):reset(x,y)
170 n.y:n.x
180 p.at4,"The coordinates of the graphics block are >>-->>";
190 p.at 0;
200 g.200
10 cls
20 for x=1 to 126
30    for y=1 to 46
40      if ((int(x/16) * 16-x)<>0) * ((int(y/6) * 6-y)<>0)then 60
50      set(x,y)
60    next y
70 next x
99 goto 99
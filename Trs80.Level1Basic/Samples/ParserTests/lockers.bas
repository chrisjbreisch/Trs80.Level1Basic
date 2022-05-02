10  cls
20  for s = 1 to 6144
30    for l = s to 6144 step s
35      m = l - 1
40      y = int(m / 128)
50      x = m - y * 128
60      if point(x, y) = 1 then 90
70      set (x, y)
80      goto 100
90      reset (x, y)
100   next l
110 next s
︠1c6a397e-c13d-459e-b754-f88e617d79d1s︠
x, y = var('x y')
_C = var('_C')
Y = function('Y')(x)

forget()

Test = 1
if Test == 0:
    dxx = x; dxy = 2*x; dyy = (x-1)
    dx = 0; dy = 0
    d = 0
    eqForX = x < 0
    eqForY = Y(x) == Y(x)
elif Test == 1:
    dxx = Y(x)^2; dxy = 2*x*Y(x); dyy = -3*x^2
    dx = -Y(x)^2/x; dy = 3*x^2/Y(x)
    d = 0
    eqForX = x > 0
    eqForY = Y(x) > 0

m = matrix(2, 2, [dxx,dxy/2, dxy/2,dyy])
det = m.det().simplify_full()
show("det", m.subs(Y(x) == y), " = ", det.subs(Y(x) == y))

print 'Характеристическое уравнение: '
cEq = dxx * diff(Y,x)^ 2 - dxy * diff(Y,x) + dyy == 0; show(cEq.subs(Y(x) == y))

print 'Решили диффур: '
ODEs = solve(cEq, diff(Y,x)); show(ODEs)

dlh0 = solve( [det < 0,  eqForX, eqForY ], x,y)
deq0 = solve( [det == 0, eqForX, eqForY ], x,y)
dgt0 = solve( [det > 0,  eqForX, eqForY ], x,y)

if len(dlh0) > 0:
    print 'det < 0'
    show('det < 0,', dlh0)
    #assume(dlh0)

    f1(x,y,_C) = desolve(ODEs[0], Y).subs(Y(x) == y)
    f2(x,y,_C) = desolve(ODEs[1], Y).subs(Y(x) == y)
    show("y1 = ",f1, "; y2 = ", f2)

    try:
        u = solve(y == f1, _C)[0]; u = u.rhs()
        v = solve(y == f2, _C)[0]; v = v.rhs()
    except:
        u = solve(f1, _C)[0]; u = u.rhs()
        v = solve(f2, _C)[0]; v = v.rhs()

elif len(deq0) > 0:
    print 'det == 0'
    show('det == 0,', deq0)
    assume(deq0)

    f1(x,y,_C) = desolve(ODEs[0], Y)
    f2(x,y,_C) = Y(x)                                # <= тут можно указать свою ф-ю, если det == 0
    show("y1 = ",f1, "; y2 = ", f2)

    try: u = solve(Y(x) == f1, _C)[0]; u = u.rhs().subs(Y(x) == y)
    except: u = solve(f1, _C)[0]; u = u.rhs().subs(Y(x) == y)
    v = f2; v = v.subs(Y(x) == y)

elif len(dgt0) > 0:
    print 'det:'
    show('det > 0,', dgt0)
    assume(dgt0)
    f1(x,y,_C) = desolve(ODEs[0], Y)
    f2(x,y,_C) = desolve(ODEs[1], Y)
    show("y1 = ",f1, "; y2 = ", f2)

    try:
        t1 = solve(Y(x) == f1, _C)[0]; t1 = t1.rhs().subs(Y(x) == y)
        t2 = solve(Y(x) == f2, _C)[0]; t2 = t2.rhs().subs(Y(x) == y)
    except:
        t1 = solve(f1, _C)[0]; t1 = t1.rhs()
        t2 = solve(f2, _C)[0]; t2 = t2.rhs()

    u = (t1 + t2).real() / 2
    v = (t1 - t2).imag() / 2


print 'Получили такую замену:'
show(u.simplify_full())
show(v.simplify_full())

print 'Якобиан замены:'
matrix(2,2, [diff(u,x), diff(u,y), diff(v,x), diff(v,y)]).det().simplify_full().show()

try:
    U = function('U')(u,v)
except:
    U = function('U')(u(x,y),v(x,y))

print 'Всякая фигня:'
Ux = U.diff(x); show("U_x = ", Ux.simplify_full())
Uy = U.diff(y); show("U_y = ", Uy.simplify_full())
Uxx = Ux.diff(x); show("U_xx = ", Uxx.simplify_full())
Uxy = Ux.diff(y); show("U_xy = ", Uxy.simplify_full())
Uyy = Uy.diff(y); show("U_yy = ", Uyy.simplify_full())

print 'Канонический вид:'
t = (dxx*Uxx + dxy*Uxy + dyy*Uyy + dx*Ux + dy*Uy + d*U).subs(Y == y) == 0;
t = t.simplify_full().expand(); t.show()
︡622bda8d-b58e-473e-82bb-b58a9e2faf5e︡︡{"html":"<div align='center'>det $\\displaystyle \\left(\\begin{array}{rr}\ny^{2} &amp; x y \\\\\nx y &amp; -3 \\, x^{2}\n\\end{array}\\right)$  =  $\\displaystyle -4 \\, x^{2} y^{2}$</div>"}︡{"stdout":"Характеристическое уравнение: \n"}︡{"html":"<div align='center'>$\\displaystyle y^{2} D[0]\\left(Y\\right)\\left(x\\right)^{2} - 2 \\, x y D[0]\\left(Y\\right)\\left(x\\right) - 3 \\, x^{2} = 0$</div>"}︡{"stdout":"Решили диффур: \n"}︡{"html":"<div align='center'>[$\\displaystyle D[0]\\left(Y\\right)\\left(x\\right) = -\\frac{x}{Y\\left(x\\right)}$, $\\displaystyle D[0]\\left(Y\\right)\\left(x\\right) = \\frac{3 \\, x}{Y\\left(x\\right)}$]</div>"}︡{"stdout":"det < 0\n"}︡{"html":"<div align='center'>det &lt; 0, [[$\\displaystyle 0 &lt; x$, $\\displaystyle x \\neq 0$, $\\displaystyle Y\\left(x\\right) \\neq 0$, $\\displaystyle Y\\left(x\\right) &gt; 0$]]</div>"}︡{"html":"<div align='center'>y1 =  $\\displaystyle \\left( x, y, C \\right) \\ {\\mapsto} \\ -\\frac{1}{2} \\, y^{2} = \\frac{1}{2} \\, x^{2} + C$ ; y2 =  $\\displaystyle \\left( x, y, C \\right) \\ {\\mapsto} \\ \\frac{1}{6} \\, y^{2} = \\frac{1}{2} \\, x^{2} + C$</div>"}︡{"stdout":"Получили такую замену:\n"}︡{"html":"<div align='center'>$\\displaystyle -\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}$</div>"}︡{"html":"<div align='center'>$\\displaystyle -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}$</div>"}︡{"stdout":"Якобиан замены:\n"}︡{"html":"<div align='center'>$\\displaystyle -\\frac{4}{3} \\, x y$</div>"}︡{"stdout":"Всякая фигня:\n"}︡{"html":"<div align='center'>U_x =  $\\displaystyle -x {\\left(D[0]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right) + D[1]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right)\\right)}$</div>"}︡{"html":"<div align='center'>U_y =  $\\displaystyle -\\frac{1}{3} \\, y {\\left(3 \\, D[0]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right) - D[1]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right)\\right)}$</div>"}︡{"html":"<div align='center'>U_xx =  $\\displaystyle x^{2} {\\left(D[0, 0]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right) + 2 \\, D[0, 1]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right) + D[1, 1]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right)\\right)} - D[0]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right) - D[1]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right)$</div>"}︡{"html":"<div align='center'>U_xy =  $\\displaystyle \\frac{1}{3} \\, x y {\\left(3 \\, D[0, 0]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right) + 2 \\, D[0, 1]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right) - D[1, 1]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right)\\right)}$</div>"}︡{"html":"<div align='center'>U_yy =  $\\displaystyle \\frac{1}{9} \\, y^{2} {\\left(9 \\, D[0, 0]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right) - 6 \\, D[0, 1]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right) + D[1, 1]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right)\\right)} - D[0]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right) + \\frac{1}{3} \\, D[1]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right)$</div>"}︡{"stdout":"Канонический вид:\n"}︡{"html":"<div align='center'>$\\displaystyle \\frac{16}{3} \\, x^{2} y^{2} D[0, 1]\\left(U\\right)\\left(-\\frac{1}{2} \\, x^{2} - \\frac{1}{2} \\, y^{2}, -\\frac{1}{2} \\, x^{2} + \\frac{1}{6} \\, y^{2}\\right) = 0$</div>"}︡{"done":true}
︠380612fe-e553-4733-8589-57067171447e︠










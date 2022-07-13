from tkinter import *
import random
from math import *
#Особо важные переменые
pole=[]
gamepole=[]
maxy = 20
maxx = 40
maxbomb = 100
bomb = maxbomb
reg = -1
#функции
def reset():
    global reg
    reg=-1
    for Ip in range(0,maxy):
        for i in range(0,maxx):
            gamepole[Ip][i]="_"
    pror()
def pror_pole(Y,X):
    global bomb
    global pole
    global gamepole
    global reg 
    bomb = maxbomb
    pole=[]
    for Ip in range(0,maxy):
        pole.append([])

        for i in range(0,maxx):
            pole[Ip].append("0") 

    gamepole=[]
    for Ip in range(0,maxy):
        gamepole.append([])
        for i in range(0,maxx):
            gamepole[Ip].append("_")    

    pole[Y][X]="+"
    pz=[]
    gpz=[]
    pz=gp(Y,X)
    for i in range(0,len(pz),2):
        
        gpz.extend(gp(pz[i],pz[i+1]))
    for i in range(0,len(gpz),2):
        pole[gpz[i]][gpz[i+1]]="+"
    reg=0
    for i in range(0,bomb):
        l=0
        while l==0:

            y=random.randint(0,maxy-1)
            x=random.randint(0,maxx-1)
            if pole[y][x]!="*" and pole[y][x]!="+":
                pole[y][x]="*"
                l=1
    for i in range(0,len(gpz),2):
        pole[gpz[i]][gpz[i+1]]="0"

    for Ip in range(0,maxy):
        for i in range(0,maxx):
            c=0
            if pole[Ip][i]!="*":
                pz=[]
                pz=gp(Ip,i)
                for co in range(0,len(pz),2):
                    if pole[pz[co]][pz[co+1]]=="*":
                        
                        c=c+1
                if c!=0:
                    pole[Ip][i]=str(c)
    
    labelbomb.configure(text=str(bomb),bg="#bd2507")
    clic(Y,X)
def ot(y,x):
    pz=[]
    pz=gp(y,x)
    for i in range(0,len(pz),2):
        if gamepole[pz[i]][pz[i+1]]!="." and gamepole[pz[i]][pz[i+1]]!="!":
            gamepole[pz[i]][pz[i+1]]=pole[pz[i]][pz[i+1]]
    pror()    
def megaot():
    l=1
    while l!=0:
        l=0
        for Ip in range(0,maxy):
            for i in range(0,maxx):
                if gamepole[Ip][i]=="0":
                    l+=1
                    pz=[]
                    pz=gp(Ip,i)
                    for co in range(0,len(pz),2):
                        if gamepole[pz[co]][pz[co+1]]!="." and gamepole[pz[co]][pz[co+1]]!="!":
                            gamepole[pz[co]][pz[co+1]]=pole[pz[co]][pz[co+1]]
                    gamepole[Ip][i]="."
    pror()
def prorot(y,x):
    c=0
    pz=[]
    pz=gp(y,x)
    for i in range(0,len(pz),2):
        if gamepole[pz[i]][pz[i+1]]=="!":
            c=c+1
            

    if gamepole[y][x]==str(c):
        ot(y,x)     
def prov():
    global reg
    o=0
    for Ip in range(0,maxy):
        for i in range(0,maxx):
            if gamepole[Ip][i]=="_":
                o=o+1
    if o==0 and bomb == 0:
        reg=2
        labelbomb.configure(text="Победа!!!",bg="#FDFC00")
def bombpr():
    for Ip in range(0,maxy):
        for i in range(0,maxx):
            if pole[Ip][i]=="*":
                gamepole[Ip][i]=pole[Ip][i]
    labelbomb.configure(text="Game over")
    pror()
def pror():
    global reg
    for Ip in range(0,maxy):
        for i in range(0,maxx):
            if gamepole[Ip][i]=="0":
                megaot()
                return 0
            if gamepole[Ip][i]=="_":
                geogpole[Ip][i].configure(text="",relief="raised",bg="#DBDBDB")
            elif gamepole[Ip][i]==".":
                geogpole[Ip][i].configure(text="",relief="sunken",bg="#C8C8C8")
            elif gamepole[Ip][i]=="!":
                geogpole[Ip][i].configure(text="!",relief="raised",bg="#EFB811")
            elif gamepole[Ip][i]=="*":
                geogpole[Ip][i].configure(text="*",relief="sunken",bg="red")
                if reg==1 or reg==0:
                    reg=2
                    bombpr()
                
                
            else:
                geogpole[Ip][i].configure(text=gamepole[Ip][i],relief="sunken",bg="#C8C8C8")
    prov()
def rclic():
    global reg   
    if reg == 0:
        reg = 1
        Lebalreg.configure(text="Стоит режим: Флашков")
    elif reg == 1:
        reg = 0
        Lebalreg.configure(text="Стоит режим: Открытия")
def gclic(y,x):
    global reg
    if reg==-1:
        pror_pole(y,x)
    if gamepole[y][x]==pole[y][x]:
        prorot(y,x)
    elif reg == 0:
        if gamepole[y][x]!="!":
            clic(y,x)
    elif reg == 1:
        
        pclic(x,y)
def pclic(x,y):

    global bomb
    if gamepole[y][x]=="_":
        gamepole[y][x]="!"
        bomb=bomb-1
    elif gamepole[y][x]=="!":
        gamepole[y][x]="_"
        bomb=bomb+1

    
    labelbomb.configure(text=str(bomb))
    if bomb<0:
        labelbomb.configure(text=str(bomb*-1)+" лишних")
    pror()
def clic(y,x):
    gamepole[y][x]=pole[y][x]
    pror()
def gp(y,x):
    ppz=[]
    pz=[]
    for i in range(-1,2):
        for Ip in range(-1,2):
            ppz.append(y+i)
            ppz.append(x+Ip)
    ppz[8]="x"
    ppz[9]="x"
    if x==0:
        ppz[0]=ppz[1]=ppz[6]=ppz[7]=ppz[13]=ppz[12]="x"
    if y==0:
        ppz[0]=ppz[1]=ppz[2]=ppz[3]=ppz[4]=ppz[5]="x"
    if y==maxy-1:
        ppz[12]=ppz[13]=ppz[14]=ppz[15]=ppz[16]=ppz[17]="x"
    if x==maxx-1:
        ppz[4]=ppz[5]=ppz[10]=ppz[11]=ppz[16]=ppz[17]="x"

    for i in range(0,18):
        if ppz[i]!="x":
            pz.append(ppz[i])
    return pz

#прорисовка всего графического
root = Tk()
root.title("Super")
root["bg"] = "#ce2706"



labelbomb=Label(root,width=40,height=1,text=str(bomb),bg="#bd2507",)
labelbomb.pack()

poleFrame=[]
for i in range(0,maxy):
    poleFrame.append('s'+str(i))
    poleFrame[i]=Frame(root)    
    poleFrame[i].pack()

geogpole=[]


         
for Ip in range(0,maxy):
    geogpole.append([])

    for i in range(0,maxx):
        geogpole[Ip].append('but'+str(Ip)+str(i)) 
        geogpole[Ip][i]=Button(poleFrame[Ip], width=2,height=1,bg="#DBDBDB" ,command=lambda i=i, Ip=Ip: gclic(Ip,i))
        geogpole[Ip][i].pack(side=LEFT)
Lebalreg=Button(root,width=40,height=1,text="Стоит режим: Открытия",bg="#bd2507",relief="flat",command= rclic)
Lebalreset=Button(root,width=40,height=1,text="reset",bg="#bd2507",relief="flat",command= reset)
Lebalreset.pack(side=LEFT)
Lebalreg.pack(side=LEFT)
#Создание поля







            

root.mainloop()





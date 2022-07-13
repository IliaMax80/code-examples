#Данное ПО было разработано под ТЗ моего папы и сейчас непосредствено им пременяетья 
#Если в общем программа по определеному синтексису ищет файлы с одним названием и разными датами, а после перемещает более старый документ в определеную папку
import os
import shutil


class File:
    global inf 
    def __init__(self, name, directory):
        self.fileName = name
        self.directory = directory 

    def chek(self):
        
        if self.fileName.find(" до ") != -1 or self.fileName.find(" от ") != -1:
            if self.fileName.find(" от ") != -1:
                self.name = self.fileName[0:self.fileName.find(" от ")]
                self.date = self.fileName[self.fileName.find(" от ")+4:-4] 
               
            else:
                #print("\n", self.fileName.rindex(" до "))
                self.name = self.fileName[0:self.fileName.rindex(" до ")]
                self.date = self.fileName[self.fileName.rindex(" до ")+4:-4]

            #print(self.date)
            if len(self.date) != 10:
                self.date = self.date[:-1]
            return 1
            
        else:
            return 0
    def dateSet(self):
        #01.01.2022  
        #if
        a = [int(str(self.date)[:2]), int(str(self.date)[3:5]), int(str(self.date)[6:10])]
        #print(*a)
        return a
    def CheckingTheNumber(self):
        a = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", " ", "." ]
        #for i in range()

    def comparison(self, dateSet):
        #04.08.2012
        
        myDataSet = self.dateSet()
        #print(myDataSet)
        if myDataSet[2] < dateSet[2]:
            #print("прошлый год")
            return 1
        elif myDataSet[1] < dateSet[1] and myDataSet[2] <= dateSet[2]:
            #print("прошлый месяц")
            return 1
        elif myDataSet[0] < dateSet[0] and myDataSet[1] <= dateSet[1] and myDataSet[2] <= dateSet[2]:
            #print("прошлый число")
            return 1
        else:
            #print("ХА")
            return 0 
    def importFile(self):
        global inf 
        fileA = os.path.join(self.directory, self.fileName)
        fileBp = os.path.join(self.directory, "Delet")
        fileBf = os.path.join(fileBp, self.fileName)
        #print(fileA, "in", fileBp)
        if os.path.exists(fileBf):
            print("Путь " + fileBf + " уже существует из-за чего файл " + self.fileName + " был удален")
            os.remove(fileA)
            return True 
            

        else:
            shutil.move(fileA, fileBp)
            return False
def fileCleaning(directory):
    global inf

    print("\n\nПроизводим чистку в файле:", directory)
    #input()
    
    catalog = []
    catalogDoc = []
    catalogimport = []
    catalogDelet = []
    
    if not os.path.exists(directory):
        print("Ошибка: директория не найдена")
        inf = inf + '\nФайл "' + str(directory) + '" не был найден'
        return
    catalog = os.listdir(directory)

    print("\nВ папке найдено", len(catalog),"файлов")
    for i in range(len(catalog)):
        file = File(catalog[i], directory) 
        catalog[i] = file
        if catalog[i].chek():
            catalogDoc.append(catalog[i])

    print("\nИз них распознано", len(catalogDoc), "Файлов ")
    for i in range(len(catalogDoc)):
        print(str(i+1) + ". " + str(catalogDoc[i].fileName)[:-4])


    for I in range(len(catalogDoc)):
        for i in range(len(catalogDoc)):
            if i != I:
                if(catalogDoc[i].name == catalogDoc[I].name):
                    #print("#Найдено совпадение")
                    if catalogDoc[I].comparison(catalogDoc[i].dateSet()):
                        #print("#Я пожалуй удалю фаил:", catalogDoc[I].fileName)
                        catalogimport.append(catalogDoc[I])


    print("\nДля инпорта выбранно "+str(len(catalogimport))+" файлов")
    for i in range(len(catalogimport)):
        print(str(i+1)+". "+catalogimport[i].fileName)

    if os.path.exists(os.path.join(directory, "Delet")) != 1:
        print("\nПримечание дириктория Delet не была найдена")
        os.mkdir(os.path.join(directory, "Delet"))

    for i in range(len(catalogimport)):
        if catalogimport[i].importFile():
            catalogDelet.append(catalogimport[i])

    inf1 = '\nВ файле "' + str(directory) + '" найдено ' + str(len(catalogDoc)) + " файлов и " 
    inf2 = str(len(catalogimport)-len(catalogDelet)) + " из них были импортированы и " + str(len(catalogDelet)) + " удалены" 
    inf = inf + inf1 + inf2 

inf = "\n\nИтоги: "
directory = os.getcwd()
print("Работаем с корневой папкой:", directory)

if os.path.exists(os.path.join(directory,'Navicleaning.txt')):

    fileNames = open(os.path.join(directory,'Navicleaning.txt'), encoding='utf-8')
    filelist = []
    while True:
        line = fileNames.readline()
        if line:
          filelist.append(line.rstrip())
        else:
            break
    for i in range(len(filelist)):
        print(str(i+1)+". "+filelist[i])
    a = []
    print("Видите через пробел номера директорий для чистки")
    print("Или нажмите enter для чистки всех файлов")
    a = input().split()

    if a != []:
        for i in range(len(a)):
            a[i] = int(a[i])-1
            fileCleaning(os.path.join(directory, filelist[a[i]]))
    else:
        for i in range(len(filelist)):
            fileCleaning(os.path.join(directory, filelist[i]))

    fileNames.close()
else:
    print("Фаил Navicleaning.txt не был найден")

print(inf)
input("\nПодробнее смотрите выше\n\nНажмите enter для закрытие терминала")


# scan source files and add to csprj.

import os
import os.path
import sys
import string
import shutil


def scan_folder(folder_path, filePathList):
	abs_src_path = os.path.abspath('.') + '\\' + folder_path
	for file in os.listdir(abs_src_path):
		sourceFile = os.path.join(abs_src_path,  file)
		if os.path.isfile(sourceFile):
			if sourceFile.find('.cs') > 0:
				filePathList.append("    <Compile Include=\"%s\\%s\" />\n" % (folder_path, file))
				#print("cs: " + "<Compile Include=\"%s\\%s\" />\n" % (folder_path, file));
				#print(gen_text)
		else:
			scan_folder(folder_path + '\\' + file, filePathList)
			#print("dir: " + (folder_path + '\\' + file));
		pass
	pass
pass


def write_csprj(file_name, gen_text):
	findItemGroup = False
	f = open(os.path.abspath('.') + "/" + file_name,'r')
	lines = f.readlines()
	startIndex = 0
	endIndex = 0
	tmpIndex = 0
	for line in lines:
		if not findItemGroup:
			find_indx = line.find('<ItemGroup>')
			if find_indx != -1:
				findItemGroup = True
				startIndex = tmpIndex
		else:
			find_indx = line.find('.cs\"')
			if find_indx != -1:
				pass
			elif tmpIndex == startIndex + 1:
				findItemGroup = False
			else:
				endIndex = tmpIndex
				break
		pass
		tmpIndex = tmpIndex + 1
	pass
	#print('start %d' % startIndex)
	#print('end %d' % endIndex)
	
	new_file_text = ""
	needInsert = True
	for i in range(0, len(lines)):
		if i <= startIndex or i >= endIndex:
			new_file_text = new_file_text + lines[i]
		else:
			if needInsert:
				needInsert = False
				new_file_text = new_file_text + gen_text
			pass
		pass
	pass
	open(os.path.abspath('.') + '/' + file_name, 'wb').write(new_file_text)
	
pass

###

dest_filePathList = []
scan_folder("Properties", dest_filePathList)
scan_folder("App", dest_filePathList)

dest_gen_text = ""
for i in range(0, len(dest_filePathList)):
	dest_gen_text = dest_gen_text + dest_filePathList[i]
pass

#print("ret: " + dest_gen_text)

write_csprj("Float.csproj", dest_gen_text);
	
	






import os
import sys
import time
import shutil

# src_filelist = None
dst_filelist = None
ignore_files = [".DS_Store"]

def update(src, dst):
    mtime = get_mtime(src)
    print("{2}: updating {0}->{1}".format(src, dst, mtime))
    if os.path.exists(dst):
        os.remove(dst)
    shutil.copy2(src, dst)
    dst_filelist[dst] = mtime

def compare(src, dst):
    for path in os.listdir(src):
        if path in ignore_files:
            continue
        joined_src_path = os.path.join(src, path)
        joined_dst_path = os.path.join(dst, path)
        # Check if exists in src
        # get time, check if greater than dst
        # If greater, overwrite and update dst time 
        if os.path.isdir(joined_src_path):
            compare(joined_src_path, joined_dst_path)
        elif os.path.isfile(joined_src_path):
            if joined_dst_path not in dst_filelist:
                update(joined_src_path, joined_dst_path)
            else:
                mtime = get_mtime(joined_src_path)
                if mtime > dst_filelist[joined_dst_path]:
                    update(joined_src_path, joined_dst_path)

def get_mtime(file):
    return os.stat(file).st_mtime

def populate_filelists(dir, dictionary=None):
    if dictionary == None:
        dictionary = dict()
    for path in os.listdir(dir):
        if path in ignore_files:
            continue
        joined_path = os.path.join(dir, path)
        if os.path.isdir(joined_path):
            dict_result = populate_filelists(joined_path, dictionary=dictionary)
        elif os.path.isfile(joined_path):
            dictionary[joined_path] = get_mtime(joined_path)
    return dictionary

if __name__ == "__main__":
    if len(sys.argv) != 3:
        print("USAGE python {} src_directory dest_directory")
        exit(1)
    src = sys.argv[1]
    dest = sys.argv[2]
    if not os.path.isdir(src):
        raise Exception("Source directory is not a directory")
    if not os.path.isdir(dest):
        raise Exception("Destination directory is not a directory")
    # src_filelist = populate_filelists(src)
    dst_filelist = populate_filelists(dest)
    while(True):
        try:
            compare(src, dest)
        except KeyboardInterrupt:
            exit(0)
        time.sleep(5)
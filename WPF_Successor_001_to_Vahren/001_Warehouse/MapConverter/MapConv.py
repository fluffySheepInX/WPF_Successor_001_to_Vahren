# ローガントゥーガ と同じく MIT License です。
# 自由に使って、改造して、配布して、いいです。
# Copyright (c) 2022 Yutaka Sawada
# Released under the MIT license

import sys
import os
import struct

# 引数の個数をチェックする
if len(sys.argv) < 2:
    print("ファイルを指定してください。")
    sys.exit()

# 複数のファイルを指定したら順番に処理する
for idx, arg in enumerate(sys.argv[1:]):

    # マップファイル(.map) のパス
    map_path = arg
    if os.path.isfile(map_path) == False:
        print(map_path + "\nというファイルは存在しません。")
        continue
    print("MAP file = " + map_path)

    # 拡張子をチェックする
    root, ext = os.path.splitext(map_path)
    if ext.lower() != '.map':
        print(map_path + "\nはマップファイルではありません。")
        continue

    # ファイルをバイナリモードで読み込む
    print("Reading ...")
    f = open(map_path, 'rb')
    data = f.read()
    # ファイルを閉じる
    f.close()

    # マップサイズを読み取る
    map_width = 0
    map_height = 0
    if len(data) > 2:
        (map_width, map_height) = struct.unpack_from('BB', data, 0)
    if map_width < 1 or map_height < 1: # サイズが小さすぎる場合はエラー
        print("マップの読み込みに失敗しました。")
        continue
    #print(map_width, map_height)

    # 地形 field の配列
    map_field = []

    # タイル情報を読み取る
    add_count = 0
    item_start = 2
    offset = 2
    file_size = len(data)
    while offset < file_size:
        if data[offset] == 0xFF:
            # タイル情報の終端を発見した
            map_field.append(None)
            add_count += 1

            # タイル内の項目の先頭バイトを探す
            while data[item_start] != 0xFF:
                if data[item_start] == 0: # field
                    text_start = item_start + 1
                    text_length = text_start + 1
                    while text_length < offset:
                        if data[text_length] == 0xFE:
                            break
                        text_length += 1
                    item_start = text_length + 1
                    text_length -= text_start
                    #print("start, length =", text_start, text_length)
                    struct_text = struct.unpack_from(str(text_length)+'s', data, text_start)
                    b_text = struct_text[0]
                    s_text = b_text.decode()
                    #print(len(map_field) - 1, 'field =', s_text)
                    map_field[-1] = s_text

                else: # それ以外の項目は無視する
                    text_start = item_start + 1
                    text_length = text_start + 1
                    while text_length < offset:
                        if data[text_length] == 0xFE:
                            break
                        text_length += 1
                    item_start = text_length + 1
                    text_length -= text_start
                    struct_text = struct.unpack_from(str(text_length)+'s', data, text_start)
                    b_text = struct_text[0]
                    s_text = b_text.decode()
                    #print(len(map_field) - 1, 'other =', s_text)

            item_start += 1

        offset += 1

    # 読み込んだタイル数をチェックする
    if add_count != map_width * map_height:
        map_field.clear()
        print("マップの読み込みに失敗しました。")
        continue

    # field element のリストを作る
    map_element = []
    for elem in map_field:
        # 要素が存在しなければリストに登録する
        if (elem in map_element) == False:
            map_element.append(elem)
            #print(elem)

    # 保存するファイルは拡張子を .txt に変更する
    save_path = root + '.txt'
    print("Writing ...")

    # ファイルをテキストモードで書き込む
    f = open(save_path, 'wt', encoding='utf-8')
    f.write("map " + os.path.basename(root) + "\n{\n")

    # field element のリストを書き込む
    offset = 0
    for elem in map_element:
        f.write("ele" + str(offset) + " = \"" + elem + "\";\n")
        offset += 1

    # field data を書き込む
    f.write("data = \"\n")
    offset = 0
    for elem in map_field:
        f.write("ele" + str(map_element.index(elem)) + ",")
        offset += 1

        # マップの横幅に達したら終端文字 @ を書き込む
        if offset == map_width:
            f.write("@,\n")
            offset = 0

    f.write("\";\n}\n")

    # ファイルを閉じる
    f.close()
    print("Converted !\n")

    # 配列を開放する
    map_field.clear()
    map_element.clear()

# 下の行をコメント解除したら、Enterキーを押すまで待ちます。
#input('Press [Enter] key to continue . . .')

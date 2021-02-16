import socket
import datetime

sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
server_address = ("10.0.0.4", 8000)
sock.bind(server_address)
print("Server started- Szerver elindult")

dict_clients = {}
dict_senders = {}
dict_rec = {}

while True:
    try:
        timestamp=datetime.datetime.now()
        #client connects
        print("__________START_________")
        print("clients:")
        print(dict_clients.keys())
        print("senders:")
        print(dict_senders)
        print("receivers:")
        print(dict_rec)
        data, client_addr = sock.recvfrom(65000)
        print("_________________________")
        print("current client:")
        print(client_addr)
        print("_________________________")
        if not dict_clients.get(client_addr):
            if len(data.strip())>20:
                print("Sorry, wrong dataformat")
                raise Exception("Sorry, wrong dataformat")
            data.strip() # e.g. "1234|sender"
            splitdata=data.split("|") # e.g. [1234,sender]
            token=splitdata[0]
            mode=splitdata[1]
            if mode=="sender":
                if dict_senders.values().count(token)<1:
                    dict_senders[client_addr]=token
                    dict_clients[client_addr]=timestamp
            elif mode=="receiver":
                if dict_rec.get(token):
                    rec_array = dict_rec.get(token)
                    if rec_array.count(client_addr)<1:
                        rec_array.append(client_addr)
                        dict_clients[client_addr]=timestamp
                    dict_rec[token]=rec_array
                else:
                    rec_array = [client_addr]
                    dict_rec[token] = rec_array
                    dict_clients[client_addr]=timestamp
                if not dict_senders:
                    sock.sendto("ures",client_addr)
                    print("Ures")
        else:
            if not dict_senders:
                sock.sendto("ures",client_addr)
                print("Ures")
            dict_clients[client_addr]=timestamp
            for i in dict_clients.keys():
                if (timestamp - dict_clients.get(i)).seconds > 6:
                    del dict_clients[i]
                    if dict_senders.get(i):
                        del dict_senders[i]
                    break
            send_token = dict_senders.get(client_addr)
            if send_token:
                print("token:")
                print(send_token)
                if dict_rec.get(send_token):
                    rec_array = dict_rec[send_token]
                    temp_array = rec_array
                    print("temp.array:")
                    print(temp_array)
                    for i in rec_array:
                        if dict_clients.get(i):
                            if (timestamp - dict_clients[i]).seconds > 6:
                                temp_array.remove(i)
                                del dict_clients[i]
                        else:
                            temp_array.remove(i)
                    dict_rec[send_token]=temp_array
                    for i in temp_array:
                        sock.sendto(data, i)
                        print("__________SENT TO_________")
                        print(i)
    except KeyboardInterrupt:
        print("Closing the system")
        sock.close()
        exit()
    except:
        print("An exception occurred")
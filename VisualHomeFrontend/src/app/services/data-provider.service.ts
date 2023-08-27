import { Injectable } from '@angular/core';
import { Subject } from 'rxjs';
import {WebSocketSubject, webSocket} from "rxjs/webSocket"
import { sleep } from 'src/utils/sleep';

@Injectable({
  providedIn: 'root'
})
export class DataProviderService {

  // TODO: Add check for existing connection and dont create a new one in case of existing

  socket?: WebSocketSubject<string>;
  messages$ = new Subject<string>();  
  connectionOpen: boolean = false;
  constructor() { 
    console.log("DataProviderService constructed");
  }



  public connect() {

    if (this.connectionOpen) return;    
    try {
      console.log("Connect called");
      this.socket = webSocket({
        url: "wss://localhost:7171/api/ws",
        openObserver: {
          next: () => this.onConnectionOpen(),
          error: (error) => {
            console.log("Error on opening websocket.", error);            
          },          
        },
        closeObserver: {
          next: (event) => {
            this.connectionOpen = false;
            console.log(`Websocket connection closed. Code: ${event.code} Reason: ${event.reason}`);            
          }
        }
      });
      
      this.socket.subscribe({
        next: message => this.onMessageFromServer(message),
        complete: () => console.log("Websocket connection complete."),
        error: (error) => {
          console.log("Error in websocket connection", error);
        }
      });


    } catch (error) {
      console.log("Error during websocket connection.", error);
    }
  }

  private onConnectionOpen() {
    this.connectionOpen = true;
    console.log("Websocket connection opened.");
    this.sendAuthToken();
    this.sendHeartBeatContinously();
  }

  public disconnect() {
    this.socket?.complete();
  }

  private onMessageFromServer(message: string) {
    console.log("Message from server:", message);
    this.messages$.next(message);
  }

  private sendAuthToken() {
    const token = localStorage.getItem("AuthToken") ?? "";    
    this.socket?.next("AuthToken: " + token);
  }

  private async sendHeartBeatContinously() {    
    while (this.connectionOpen) {
      this.socket?.next("heartbeat");
      console.log(`Heartbeat sent!`);
      await sleep(2000);
    }    
  }
}

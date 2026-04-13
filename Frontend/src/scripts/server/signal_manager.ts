import * as SignalR from "@microsoft/signalr";
import server_config from "./config.json";

export enum SignalName {
    GAME_UPDATE = "GameUpdate",
    GAME_OVER = "GameOver",
    GAME_STARTED = "GameStarted",
    GAME_END = "GameEnded",
    PLAYER_INPUT = "SendInput",
    JOIN_GROUP = "JoinGroup",
    LEAVE_GROUP = "LeaveGroup"
};

export type SignalCallback = (...args: any) => any;

export default class SignalManager {
    private connection: SignalR.HubConnection;

    constructor(hub_prefix: string) {
        this.connection = new SignalR.HubConnectionBuilder()
            .withUrl(server_config.server_base_url + hub_prefix)
            .withAutomaticReconnect()
        .build();
    }

    public async start() {
        await this.connection.start();
    }

    public registerCallback(signal: SignalName, callback: SignalCallback): void {
        this.connection.on(signal, callback);
    }

    public async invokeSignal(signal: SignalName, ...args: any) {
        await this.connection.invoke(signal, ...args);
    }
}
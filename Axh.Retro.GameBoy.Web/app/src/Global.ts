class Global {
}

interface ILogService {

    debug(message: String);

    info(message: String);

    error(message: String);
}

class ConsoleLogService implements  ILogService {
    debug(message: String) {
        this.log(message, "DEBUG");
    }

    info(message: String) {
        this.log(message, "INFO");
    }

    error(message: String) {
        this.log(message, "ERROR");
    }

    private log(message: String, level: String) {
        var date = new Date().toISOString();
        console.log(`${date} ${level} - ${message}`);
    }
}

class GameBoyApiService {

    getAll(callback: (ids: string[]) => void): JQueryXHR {
        return $.get("/api/gameboy", {
            success(response) {
                var ids = <string[]> response;
                callback(ids);
            },
            error() {
                alert("error");
            }
        });
    }
}

class GameBoy {
    private id: string;

    constructor(id: string) {
        this.id = id;


    }



}
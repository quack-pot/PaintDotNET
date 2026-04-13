import server_config from "./config.json";

async function handleRequest(path: string, method: string, body?: any) {
    return await fetch(server_config.server_base_url + path, {
        method: method,
        headers: { "Content-Type": "application/json" },
        body: (body === undefined || body === null) ? undefined : JSON.stringify(body)
    });
}

const ServerRequests = {
    GET: async (path: string) => await handleRequest(path, "GET"),
    POST: async (path: string, body?: any) => await handleRequest(path, "POST", body),
    PUT: async (path: string, body?: any) => await handleRequest(path, "PUT", body),
    DELETE: async (path: string, body?: any) => await handleRequest(path, "DELETE", body)
};

export default ServerRequests;
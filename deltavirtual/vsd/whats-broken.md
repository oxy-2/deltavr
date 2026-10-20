1. The `renode-ws-proxy` is Not Running

The modern graphical tools and VS Code extensions communicate with Renode using a WebSocket proxy client (`renode-ws-proxy`). If this proxy isn't active, the UI client will show a disconnected status. [[1](https://antmicro.com/blog/2024/12/introducing-renode-vscode-extension)]

- **Fix:** Open your terminal and ensure you start the proxy server before launching your IDE or VSD client dashboard: [[1](https://antmicro.com/blog/2024/12/introducing-renode-vscode-extension)]
    
    bash
    
    ```
    pip3 install renode-ws-proxy
    renode-ws-proxy
    ```
    
    Use code with caution.
    

2. Renode is Not Running in Telnet Mode

If your client expects a background Renode session to connect to, Renode must be explicitly launched with the remote connection listener port open. [[1](https://renode.readthedocs.io/en/latest/basic/running.html), [2](https://docs.swedishembedded.com/renode-docs/basic/running.html)]

- **Fix:** Ensure you are starting Renode with the appropriate port switch (often `21234` or what is defined in your `.vscode/launch.json`): [[1](https://github.com/antmicro/renode-vscode-extension), [2](https://docs.swedishembedded.com/renode-docs/basic/running.html)]
    
    bash
    
    ```
    renode -P 21234
    ```
    
    Use code with caution.
    

3. Port Conflicts (Address Already in Use)

A previous instance of Renode may have crashed or stayed alive in the background, locking the required communications port (such as the GDB server port `3333` or the WebSocket port). [[1](https://renode.readthedocs.io/en/latest/debugging/vscode.html), [2](https://mi-v-ecosystem.github.io/SoftConsole-Documentation/SoftConsole-v2021.3/troubleshooting/renode.html)]

- **Fix:** Kill lingering processes to free up the ports:
    
    - **Linux/macOS:** `pkill -f renode` or `lsof -i :3333` (to find and kill the specific PID).
    - **Windows:** Open Task Manager and terminate any lingering `Renode.exe` or `dotnet` processes. [[1](https://mi-v-ecosystem.github.io/SoftConsole-Documentation/SoftConsole-v2021.3/troubleshooting/renode.html)]
    

4. Configuration Path Mismatches

The client looks for the exact script (`.resc`) and artifact paths defined in your environment. If the configuration points to an invalid directory, the pipeline fails silently and drops the connection. [[1](https://marketplace.visualstudio.com/items?itemName=11cookies11.renode-workbench), [2](https://github.com/antmicro/renode-vscode-extension)]

- **Fix:** Verify your `.vscode/launch.json` or VSD settings to make sure `"resc"` and `"terminals"` WebSocket URIs perfectly mirror your actual local setup. [[1](https://github.com/antmicro/renode-vscode-extension)]

To help narrow down the problem, could you tell me:

- What **Operating System** you are running?
- Are you using the **official VS Code extension**, an **experimental GUI**, or a **third-party tool**?
- Any specific **log output** you see in your Renode monitor or terminal when the disconnection happens?

## status for vsd (checked against the code)

the sections above are generic renode and vs code advice. vsd doesnt use renode-ws-proxy, port 21234, or a vs code launch.json. there are zero references to any of that in the vsd folder. renode only starts when you hit "run simulation", and pyrenode3 runs it in process.

the empty node browser is a different problem. it's the pipeline manager link on port 5000, not renode.

## status: fixed (python 3.12+ tcp wait)

the empty node browser was caused by pipeline manager's tcp server hanging on python 3.12.1+. `wait_for_client` closes the listening socket and then waits for every client to disconnect. with vsd connected, that wait never ends, so the connection lock never releases and the message reader never starts.

the fix is in `visual-system-designer-app/vsd/backend.py`. the pipeline manager process now restores the old `wait_closed()` behavior for its tcp server. the connect retries are still in there too.

if the palette goes empty again, check these first:

- are you on python 3.12+? (the venv is 3.14 on this pc)
- are there extra browser tabs open on 127.0.0.1:9000? the server only answers the newest session.

fallback if it breaks again: make the venv on python 3.11, which doesnt have this wait change.

also, the earlier "system python" theory was wrong. the child shows the base python because the venv launcher starts it. that's not the cause.

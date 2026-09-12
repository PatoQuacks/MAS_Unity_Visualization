Installation:
- Create a new 3D Unity Project
- Open your project in the Unity Editor
- Go to the top menu and select Assets > Import Package > Custom Package
- Find and select the downloaded "MAS_System.unitypackage" file in the file browser window, then click Open
- Click Import (or Import Anyway if prompted about signatures) to add the files to the project's "Assets" folder.

Execution:
- Run an instance of the project
- Console should show debug log: "NetworkReceiver listening on http://localhost:5005/"
- If error occurs, or no debug log is shown, try:
    - Stopping current instance and running it again
    - Change the server's port in the URL found in this project, and the one in MAS Python implementation script "main.py"
- Execute MAS Python implementation script "main.py"

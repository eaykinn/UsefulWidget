<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0"> 
    
  </head>

  <body>
    <h1>UsefulWidget</h1>
    <p>My first C# WPF app. The app can be used as desktop widget.</p>
    <p>Widget includes:</p>
    <ul> 
      <li>Date & Clock</li>
      <li>System Volume Controller</li>
      <li>Media Playback Control(NPSMLib)</li>
      <li> Weather Forecast (open-meteo)</li>
      <li>Countdown timer with Windows shutdown/restrart</li>
    </ul>
    <p> You can also change theme from top left corner. Checkbox below the player provides stopping music when the user locks the current Windows session. I use HandyControl library for interface and colorpicker.</p>
    <h2>Note:</h2>
     <p>You need to create api_key.json file in Resources directory and put clientId and clientSecret (for spotify api) in it with following format: 
       <p> 
    {<br>
    "clientId": "YOUR_CLIENT_ID",
         <br>
    "clientSecret": "YOUR_CLIENT_SECRET"
         <br>
    }
         <br>
     </p>
   <div>
     <h2>Maximized Window</h2>
     <picture>
      <img src="https://github.com/user-attachments/assets/3eaccf4d-9a6b-406d-a887-2cfca5d6d63a" alt="maximized" style="width:auto;">
    </picture> 
   </div> 
    <div> 
       <h2>Minimized Window</h2>
      <picture>
        <img src="![image](https://github.com/user-attachments/assets/4886c6ad-ad11-4a9a-84d6-2344a7a2a5de)
" style="width:auto;">
      </picture>
   </div>
        <div> 
       <h2>Similar Music List</h2>
          <p>Opens when double clicked on song name && a new song or artist can be searched</p>
      <picture>
        <img src = "![image](https://github.com/user-attachments/assets/7c4ce1e1-4134-4250-a675-f8c5c3a341e3)" alt="maximized" style="width:auto;">
      </picture>
   </div>
  </body>
</html>

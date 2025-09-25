<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0"> 
    
  </head>

  <body>
    <h1>UsefulWidget</h1>
    <p>My first C# WPF app (Hobby project). The app can be used as desktop widget.</p>
    <p>Widget includes:</p>
    <ul> 
      <li>Date & Clock</li>
      <li>System Volume Controller</li>
      <li>Media Playback Control(NPSMLib) for only spotify app</li>
      <li>Spotify api for searching content</li>
      <li> Weather Forecast (open-meteo)</li>
      <li>Countdown timer with Windows shutdown/restart</li>
      <li>Lyrics (by GeniusApi with web parsing)</li>
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
      <img src="https://github.com/user-attachments/assets/ff3e9225-34ee-4969-9425-675156b13dce" />
    </picture> 
   </div> 
    <div> 
       <h2>Minimized Window</h2>
      <picture>
        <img src="https://github.com/user-attachments/assets/5006c9fc-db1e-4533-a3d1-89b7536c91e8" />
 style="width:auto;">
      </picture>
   </div>
        <div> 
       <h2>Similar Music List</h2>
          <p>Opens when clicked on song name && a new song or artist can be searched</p>
      <picture>
        <img src="https://github.com/user-attachments/assets/f0abec9f-a1f4-4431-861a-a5622072cdde" />
 alt="maximized" style="width:auto;">
      </picture>
   </div>
  </body>
</html>

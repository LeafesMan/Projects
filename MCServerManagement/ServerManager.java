import java.util.*;

public class ServerManager{

	private static final int restartFrequency = 10800, milliPerSec = 1000;
	private static final int[] restartWarnings = new int[]{3600, 1800, 600, 60, 30, 10, 9, 8, 7, 6, 5, 4, 3, 2, 1};

	public static void main(String[] args)
	{
		backupLoop();
	}
	
	//Starts a backup loop
	public static void backupLoop(){

		try{
			//Wait for restart frequency seconds
			System.out.println("[MC-SERVER] Manager INIT\n[MC-SERVER] Waiting: " + restartFrequency + " seconds...");	
			Thread.sleep(restartFrequency * milliPerSec -30000);
			
			//Restart and Backup
			processRestartTime();	
			Thread.sleep(3000);
			coldBackup();

			//Recursive Backup
			backupLoop();
		}
		catch(Exception e){
			System.out.println("[MC-SERVER] CRITICAL FAILURE IN BACKUP LOOP");
		}
	}

	//Processes restart frequency
	//Gives warnings at restartwarnings times
	private static void processRestartTime(){
	
		try{	
			boolean foundWarning = false;
			//Wait till each restart warning time printing warnings
			for (int i =0; i <restartWarnings.length; i++){
				if(restartWarnings[i] <= restartFrequency){
					//If first warning found within restart frequency
					// wait until aligned with restart warnings
					if(!foundWarning){
						foundWarning = true;
						Thread.sleep( (restartFrequency - restartWarnings[i+1]) * milliPerSec);
					}
					//Get Affix based on time wawiting
					String affix = "";
					int dur = restartWarnings[i];
					if(restartWarnings[i] >= 7200) {affix ="hours"; dur/=3600;}
					else if(restartWarnings[i] >= 3600) {affix ="hour"; dur/=3600;}
					else if(restartWarnings[i] >= 120) {affix ="minutes"; dur/=60;}
					else if(restartWarnings[i] >= 60) {affix ="minute"; dur/= 60;}
					else if(restartWarnings[i] > 10) affix = "seconds";
				
					//Print time until restart in chat and wait that until next restart warning
					ServerChat.chat( ("Restarting in " + dur + " " + affix) , "red");
					if(i+1 != restartWarnings.length)Thread.sleep( (restartWarnings[i]-restartWarnings[i+1])*milliPerSec);
					else Thread.sleep(1000);
				}
			}
		}
		catch(Exception e){
			System.out.println("[MC-SERVER] PROCESS RESTART TIME FAILED");
		}
	}

	//Performs a cold back
	//Shutting down the server
	//Saving world file to backup folder
	//Starting up server
	public static void coldBackup(){
		try{
			System.out.println("MC-SERVER] STARTING BACKUP");
			Process p = Runtime.getRuntime().exec("./coldbackup");
			p.waitFor();
			System.out.println("[MC-SERVER] BACKUP COMPLETE");
		}
		catch(Exception e){
			System.out.println("[MC-SERVER] BACKUP FAILED");
		}	
	}
}

import java.util.*;

public class ServerChat{
	
	public static void main(String[] args)
	{
		//If no args or more than two ABORT We need text and color(optional)
		if(args.length != 1 && args.length != 2) return; 	
	
		//Make chat first value in args array
		if(args.length >1) chat(args[0], args[1]);
		else 		   chat(args[0], "");
	}

	public static void chat(String txt, String color){
		Hashtable<String,String> colorDict = GetColorDict();

		try{
			//Make args array for chat shell script
			String colorChar = colorDict.get(color);
			if(colorChar == null) colorChar = "F";
			String[] input = new String[]{"./chat", txt, colorChar};
			Process p = Runtime.getRuntime().exec(input);
		}
		catch (Exception e){
			System.out.println("[MC-SERVER] CHAT FAILURE ABORTING");
		}
	}
	

	public static Hashtable<String,String> GetColorDict(){
		Hashtable<String, String> d = new Hashtable<String,String>();
		d.put("black", "0");
		d.put("darkblue", "1");
		d.put("darkgreen", "2");
		d.put("darkaqua", "3");
		d.put("darkred", "4");
		d.put("darkpurple", "5");
		d.put("gold", "6");
		d.put("gray", "7");	
		d.put("darkgray", "8");
		d.put("blue", "9");
		d.put("green", "A");
		d.put("aqua", "B");
		d.put("red", "C");
		d.put("lightpurple", "D");
		d.put("yellow", "E");
		return d;
	}
}

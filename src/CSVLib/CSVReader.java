package edu.gatech.cs6310;

import java.io.IOException;
import java.nio.file.*;
import java.util.List;

/**
 * @author Alfred Johnson
 *
 *	Class used to read a CSV file. Create an object with the CSV filename,
 *	then call doRead(), providing an object to a class that implements the
 *	ICSVReader interface. The oCSVEntry method in that object will be repeatedly
 *	called for each line in the file.
 */
public class CSVReader {
	public CSVReader(String file) {
		m_file = file;
	}
	
	public void doRead(ICSVReader csvEntry) {
		try {
			Path p = Paths.get(m_file);
//			String s = p.toString();
			List<String> lines = Files.readAllLines(p);
			
			int lineCount = 0;
			for(String line : lines) {
				String[] entries = line.split(",");
				csvEntry.onCSVEntry(lineCount++, entries);
			}
		} catch (IOException e) {
		}
	}
	
	private String m_file;
}

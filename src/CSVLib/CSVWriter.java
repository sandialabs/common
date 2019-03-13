package edu.gatech.cs6310;

import java.io.IOException;
import java.nio.charset.Charset;
import java.nio.file.*;


/**
 * @author Alfred Johnson
 *
 *	Used to write a series of CSV-style lines to the specified file.
 *	Each entry in the list of lines to write should already be
 *	comma-separated.
 */
public class CSVWriter {
	public CSVWriter(String file) {
		m_file = file;
	}
	
	public void doWrite(CSV<?> csv) {
		// Currently, we don't actually write the file out, but I don't want
		// to comment this out and then get warnings about the imports, so
		// just write the code so it doesn't actually write.
		boolean write = false;
		if(write == false)
			return;

		try {
			Path file = Paths.get(m_file);
			Files.write(file, csv.allLines(), Charset.forName("UTF-8"));
		} catch (IOException e) {
		}
	}
	
	private String m_file;
}

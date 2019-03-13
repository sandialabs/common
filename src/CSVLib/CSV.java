package edu.gatech.cs6310;

import java.util.ArrayList;

/**
 * Lets you create objects of type T from a CSV file.
 * Each row in the CSV is added to the m_entries list.
 * 
 * Is an abstract class so you'll want to implement your own generate
 * and generateLine methods.
 * 
 * @author Alfred Johnson
 *
 * @param <T>
 */
public abstract class CSV<T> implements ICSVReader, ICSVWriter {
	public CSV(String filename) {
		m_filename = filename;
		m_entries = new ArrayList<T>();
	}
	
	/**
	 * Called as a T is created inside the generate method
	 * 
	 * @param t The object to insert
	 */
	protected void add(T t) {
		if(t != null)
			m_entries.add(t);
	}
	
	/**
	 * Get all of the entries in the CSV as rows of text. Is typically used as the
	 * data is being serialized back out to file.
	 * 
	 * @return The list of strings as each T object is serialized to a CSV line
	 */
	public ArrayList<String> allLines() {
		Init();
		ArrayList<String> lines = new ArrayList<>();
		for(T t : m_entries) {
			lines.add(generateLine(t));
		}
		return lines;
	}
	
	/**
	 * Generate a single T given the entries
	 * @param entries The individual pieces of one line of the CSV
	 * @return A T object
	 */
	public abstract T generate(String[] entries);
	
	/**
	 * Generate one line of the CSV from a single T object
	 * @return A string that formats T into the appropriate line of the CSV
	 */
	public abstract String generateLine(T t);
	
	/**
	 * "Callback" called as each line in the CSV file is read
	 * 
	 * @param row The 0-based row in the file
	 * @param entries The individual entries in the row
	 */
	@Override
	public void onCSVEntry(int row, String[] entries) {
		T t = generate(entries);
		add(t);
	}
	
	@Override
	public void writeFile() {
		CSVWriter writer = new CSVWriter(m_filename);
		writer.doWrite(this);
	}
	
	protected void Init() {
		if(m_entries.size() == 0) {
			CSVReader reader = new CSVReader(m_filename);
			reader.doRead(this);
		}
	}
	
	protected String m_filename;
	protected ArrayList<T> m_entries;
}

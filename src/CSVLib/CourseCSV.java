package edu.gatech.cs6310.cms.db;

import java.text.MessageFormat;
import java.util.*;

import edu.gatech.cs6310.CSVReader;
import edu.gatech.cs6310.CSVWriter;
import edu.gatech.cs6310.ICSVReader;
import edu.gatech.cs6310.ICSVWriter;
import edu.gatech.cs6310.cms.Season;

/**
 * @author Alfred Johnson
 *
 *	Represents a CSV file holding the courses
 */
public class CourseCSV implements ICSVReader, ICSVWriter {
	public CourseCSV() {
		m_courses = new HashMap<Integer, CourseEntry>();
	}
	
	public ArrayList<CourseEntry> getCourses() {
		Init();
		return new ArrayList<CourseEntry>(m_courses.values());
	}
	
	public void addCourse(CourseEntry entry) {
		Init();
		m_courses.put(entry.id, entry);
		writeFile();
	}
	
	public void updateCourse(CourseEntry entry) {
		addCourse(entry);
	}

	/**
	 * "Callback" called as each line in the CSV file is read
	 * 
	 * @param row The 0-based row in the file
	 * @param entries The individual entries in the row
	 */
	public void onCSVEntry(int row, String[] entries) {
		if(entries.length >= 2) {
			CourseEntry entry = new CourseEntry();
			entry.id = Integer.parseInt(entries[0]);
			entry.name = entries[1];
			for(int i = 2; i < entries.length; ++i) {
				Season season = Season.fromString(entries[i]);
				if(season != null)
					entry.seasons.add(season);
			}
//			System.err.println(entry.toString());
			m_courses.put(entry.id, entry);
		}
	}
	
	@Override
	public void writeFile() {
		ArrayList<String> lines = new ArrayList<>();
		for(CourseEntry entry : m_courses.values())
			lines.add(entryToLine(entry));
		CSVWriter writer = new CSVWriter(c_filename);
		writer.doWrite(lines);
	}
	
	/**
	 * Format the course entry as a CSV file. The seasons part has a variable length
	 * 
	 * @param entry The course entry to write as a CSV string
	 * @return The generated string
	 */
	private String entryToLine(CourseEntry entry) {
		String line = MessageFormat.format("{0},{1}", entry.id, entry.name);
		for(Season s : entry.seasons)
			line += MessageFormat.format(",{0}", s);
		return line;
	}
	
	private void Init() {
		if(m_courses.size() == 0) {
			CSVReader reader = new CSVReader(c_filename);
			reader.doRead(this);
		}
	}

	private HashMap<Integer, CourseEntry> m_courses;
	
	private static final String c_filename = "courses.csv";
}

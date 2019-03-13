package edu.gatech.cs6310;


/**
 * @author Alfred Johnson
 *
 *	Interface used to read CSV files. This method in your class that implements
 *	this interface will be repeatedly called for each line in a CSV file.
 */
public interface ICSVReader {
	public void onCSVEntry(int row, String[] entries);
}

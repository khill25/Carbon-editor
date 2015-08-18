//
//  SFNodeNavigationViewController.m
//  SFNode
//
//  Created by Kaili Hill on 8/7/15.
//  Copyright (c) 2015 Kaili Hill. All rights reserved.
//

#import "SFNodeNavigationViewController.h"

@interface SFNodeNavigationViewController () <NSOutlineViewDataSource, NSOutlineViewDelegate>

@end

@implementation SFNodeNavigationViewController

- (void)viewDidLoad {
    [super viewDidLoad];
    // Do view setup here.
}

#pragma mark - NSOutlineView stuff

/*
- (NSInteger)outlineView:(NSOutlineView *)outlineView numberOfChildrenOfItem:(id)item;
- (id)outlineView:(NSOutlineView *)outlineView child:(NSInteger)index ofItem:(id)item;
- (BOOL)outlineView:(NSOutlineView *)outlineView isItemExpandable:(id)item;

// NOTE: this method is optional for the View Based OutlineView.

- (id)outlineView:(NSOutlineView *)outlineView objectValueForTableColumn:(NSTableColumn *)tableColumn byItem:(id)item;
 */

/* View Based OutlineView: See the delegate method -tableView:viewForTableColumn:row: in NSTableView.
 */
//- (NSView *)outlineView:(NSOutlineView *)outlineView viewForTableColumn:(NSTableColumn *)tableColumn item:(id)item NS_AVAILABLE_MAC(10_7);

/* View Based OutlineView: See the delegate method -tableView:rowViewForRow: in NSTableView.
 */
//- (NSTableRowView *)outlineView:(NSOutlineView *)outlineView rowViewForItem:(id)item NS_AVAILABLE_MAC(10_7);

/* View Based OutlineView: This delegate method can be used to know when a new 'rowView' has been added to the table. At this point, you can choose to add in extra views, or modify any properties on 'rowView'.
 */
//- (void)outlineView:(NSOutlineView *)outlineView didAddRowView:(NSTableRowView *)rowView forRow:(NSInteger)row NS_AVAILABLE_MAC(10_7);

/* View Based OutlineView: This delegate method can be used to know when 'rowView' has been removed from the table. The removed 'rowView' may be reused by the table so any additionally inserted views should be removed at this point. A 'row' parameter is included. 'row' will be '-1' for rows that are being deleted from the table and no longer have a valid row, otherwise it will be the valid row that is being removed due to it being moved off screen.
 */
//- (void)outlineView:(NSOutlineView *)outlineView didRemoveRowView:(NSTableRowView *)rowView forRow:(NSInteger)row NS_AVAILABLE_MAC(10_7);


@end

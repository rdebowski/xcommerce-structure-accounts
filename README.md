# Structure Accounts xCommerce Plugin


## xCommerce Plugin based on 9.0.2 version

# Welcome 

I would like to provide you a module which allows you to make structure accounts in you Sitecore and Commerce engine. 
Ite means that you will have 2 types of user:
- Master1
  - Slave1
  - Slave2
  - Slave3
- Master2
  - Slave1
  - Slave2

Above list show relation between them. Master user is a parent for the slave one, or from other perspecitve we can say that slave user belongs to master user.

----------

# Technical Overview

Basically this plugin contains custom API with the methods you can use:

- GetSubaccounts
- UpdateSubaccount
- DeleteSubaccounts

At this moment Create action is made without custom API action. Currently during adding new slave account by Master user, a new information about MasterID is added to this newly created slave user, hence we know that is slave user.

## Structure relation mechanism

To make it as easy as it can be, the simpler way were to keep list of slave user just directly in master account. It solve issues with filtering.

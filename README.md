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

You will ask where is Create ?

Well it's done in different way currently. At the begining adventure with xCommerce question was: How to extend customer xCommerce entity. The faster way it was found, was to extend xCommerce json policy with custom fields and run boostrap to re-generate schema of customer entity in database.
Currently with knowledge which was gethered last months, it's possible to change e.g. composer component or just component. I believe there will be a chance and time to make it and describe deeper :) Feel free to add.

## Structure relation

To make it as easy as it can be, the simpler way were to keep list of slave user just directly in master account. It solve issues with filtering.

## What additional you can find here?

There also more then one additional field with Main Account Id, which allow to create structured relation. There also fields like:
- MainAccountName
- CanPurchase
- CanCreateOrders
- IsActive
- ConsentProcessingContactData
- CanSeeDiscountPrices
 
 There were prepared, to extend behaviors of an account. I was wondering should I keep it for you or remove it, then I ask myself but why not show them? It's better to show some more exmamples then less. So I keep it with faith it will help someone in her/his needs.


